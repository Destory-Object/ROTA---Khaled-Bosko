using System.Collections;
using UnityEngine;

///   5. Assign interactablePrefab + spawnPoint if you want loot on death (same as EnemyShooter)
///   6. Animator bools expected: "isWalking", "isAttacking", "isStunned", "isDead"

public class MeleeEnemy : MonoBehaviour, IHealth, IInteractable
{

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Attack Timing")]
    [SerializeField] private float attackWindupDuration = 0.4f;   // How long the windup lasts — parry window
    [SerializeField] private float attackActiveDuration = 0.2f;   // How long the hitbox is live

    [Header("Parry")]
    [SerializeField] private float parryStunDuration = 1.5f;
    [SerializeField] private int parryCounterDamage = 1;           // Damage enemy takes when parried

    [Header("Loot (same as EnemyShooter)")]
    [SerializeField] private GameObject interactablePrefab;
    [SerializeField] private Transform spawnPoint;


    private enum State { Idle, Chasing, WindingUp, Attacking, Stunned, Dead }
    private State state = State.Idle;

    private bool canBeParried = false;
    private float lastAttackTime = -99f;

    private Transform playerTransform;
    private PlayerInputActions playerInputActions;   // to call IsParrying()
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Find the player — same pattern as EnemyShooter
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerInputActions = player.GetComponent<PlayerInputActions>();
        }
    }

    private void Update()
    {
        if (state == State.Dead) return;

        float dist = playerTransform != null
            ? Vector2.Distance(transform.position, playerTransform.position)
            : float.MaxValue;

        switch (state)
        {
            case State.Idle:
                if (dist <= detectionRange)
                    state = State.Chasing;
                break;

            case State.Chasing:
                ChasePlayer(dist);
                break;

                // WindingUp / Attacking / Stunned are all driven by coroutines
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Dead || state == State.Stunned
            || state == State.WindingUp || state == State.Attacking)
        {
            rb.linearVelocityX = 0f;
            return;
        }

        if (state == State.Chasing && playerTransform != null)
        {
            float dir = playerTransform.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocityX = dir * moveSpeed;

            // Flip sprite to face player
            transform.rotation = Quaternion.Euler(0f, dir < 0 ? 180f : 0f, 0f);
        }
    }


    private void ChasePlayer(float dist)
    {
        animator?.SetBool("isWalking", true);

        // Lost the player
        if (dist > detectionRange * 1.5f)
        {
            state = State.Idle;
            animator?.SetBool("isWalking", false);
            return;
        }

        // Close enough to attack
        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(AttackRoutine());
        }
    }

    // ── Attack coroutine ───────────────────────────────────────────────────
    private IEnumerator AttackRoutine()
    {
        // --- WINDUP: player can parry here ---
        state = State.WindingUp;
        canBeParried = true;
        animator?.SetBool("isWalking", false);
        animator?.SetBool("isAttacking", true);

        float windupTimer = 0f;
        while (windupTimer < attackWindupDuration)
        {
            windupTimer += Time.deltaTime;

            // Player pressed parry during windup → enemy gets countered
            if (playerInputActions != null && playerInputActions.IsParrying())
            {
                canBeParried = false;
                animator?.SetBool("isAttacking", false);
                StartCoroutine(ParryStunRoutine());
                yield break;
            }
            yield return null;
        }
        canBeParried = false;

        // Interrupted by stun or death during windup
        if (state == State.Stunned || state == State.Dead)
            yield break;

        // --- ACTIVE HIT WINDOW ---
        state = State.Attacking;

        float activeTimer = 0f;
        bool hitLanded = false;

        while (activeTimer < attackActiveDuration && !hitLanded)
        {
            // Uses the same utility your other scripts use
            Collider2D[] hits = AttackUtilities.DetectEnemies(
                attackPoint.position, attackRange, playerLayer);

            foreach (Collider2D hit in hits)
            {
                // Check parry one more time — if player parried at the very last moment
                if (playerInputActions != null && playerInputActions.IsParrying())
                {
                    // Late parry — still counts
                    canBeParried = false;
                    animator?.SetBool("isAttacking", false);
                    StartCoroutine(ParryStunRoutine());
                    yield break;
                }

                // Deal damage through IHealth — same as your PlayerPogo does
                IHealth health = hit.GetComponent<IHealth>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage);
                    hitLanded = true;
                }
            }

            activeTimer += Time.deltaTime;
            yield return null;
        }

        animator?.SetBool("isAttacking", false);

        if (state != State.Dead)
            state = State.Chasing;
    }

    // ── Parry stun ─────────────────────────────────────────────────────────
    private IEnumerator ParryStunRoutine()
    {
        state = State.Stunned;
        animator?.SetBool("isStunned", true);
        animator?.SetBool("isAttacking", false);

        Debug.Log($"{name} was parried!");

        // Enemy takes counter damage
        TakeDamage(parryCounterDamage);

        yield return new WaitForSeconds(parryStunDuration);

        if (state == State.Dead) yield break;

        animator?.SetBool("isStunned", false);
        state = State.Chasing;
    }

    // ── IHealth ────────────────────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        if (state == State.Dead) return;

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void RegenHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + Mathf.Max(0, amount), maxHealth);
    }

    public int GetHealth() => currentHealth;

    public void Kill() => Die();

    private void Die()
    {
        state = State.Dead;
        StopAllCoroutines();

        animator?.SetBool("isWalking", false);
        animator?.SetBool("isAttacking", false);
        animator?.SetBool("isStunned", false);
        animator?.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Spawn loot — identical pattern to EnemyShooter
        if (interactablePrefab != null)
        {
            Vector3 lootPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(interactablePrefab, lootPos, Quaternion.identity);
        }

        Destroy(gameObject, 3f);
    }

    // ── IInteractable ──────────────────────────────────────────────────────
    /// <summary>
    /// PlayerInteraction calls this when the player presses Interact near the enemy.
    /// Tag this GameObject "Interactable" so PlayerInteraction's trigger picks it up.
    /// </summary>
    public void Interact()
    {
        switch (state)
        {
            case State.Dead:
                Debug.Log($"Looting {name}...");
                break;

            case State.Stunned:
                // Parried enemy is vulnerable — instant kill on interact
                Debug.Log($"Executed {name} while stunned!");
                Die();
                break;

            default:
                Debug.Log($"{name} growls at you.");
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}