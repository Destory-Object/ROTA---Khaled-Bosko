using System.Collections;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IHealth, IInteractable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float stopDistance = 1.2f;     // Enemy halts here — should be just outside
                                                            // meleeStartRange so it stops then attacks

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float meleeStartRange = 1.5f;
    [SerializeField] private float attackHitboxRange = 0.6f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Attack Timing")]
    [SerializeField] private float attackWindupDuration = 0.4f;
    [SerializeField] private float attackActiveDuration = 0.2f;

    [Header("Parry")]
    [SerializeField] private float parryStunDuration = 1.5f;
    [SerializeField] private int parryCounterDamage = 1;

    [Header("Loot")]
    [SerializeField] private GameObject interactablePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Attack Telegraph")]
    [SerializeField] private GameObject warningIndicator;
    [SerializeField] private Color windupColor = Color.red;
    [SerializeField] private float flashSpeed = 8f;

    private enum State { Idle, Chasing, WindingUp, Attacking, Stunned, Dead }
    private State state = State.Idle;

    private float lastAttackTime = -99f;

    private Transform playerTransform;
    private PlayerInputActions playerInputActions;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (warningIndicator != null)
            warningIndicator.SetActive(false);

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
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            float dir = playerTransform.position.x > transform.position.x ? 1f : -1f;

            // Always face the player
            transform.rotation = Quaternion.Euler(0f, dir < 0 ? 180f : 0f, 0f);

            // Stop moving once close enough — stand in front without touching
            if (dist <= stopDistance)
                rb.linearVelocityX = 0f;
            else
                rb.linearVelocityX = dir * moveSpeed;
        }
    }

    private void ChasePlayer(float dist)
    {
        bool inStopZone = dist <= stopDistance;
        animator?.SetBool("isWalking", !inStopZone);    // Stop walk animation when standing still

        // Lost the player
        if (dist > detectionRange * 1.5f)
        {
            state = State.Idle;
            animator?.SetBool("isWalking", false);
            return;
        }

        // Start attack when within melee range and off cooldown
        if (dist <= meleeStartRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            state = State.WindingUp;
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        animator?.SetBool("isWalking", false);
        animator?.SetBool("isAttacking", true);

        ShowWarning(true);
        StartCoroutine(FlashSprite());

        // WINDUP: parry window
        float windupTimer = 0f;
        while (windupTimer < attackWindupDuration)
        {
            windupTimer += Time.deltaTime;

            if (playerInputActions != null && playerInputActions.IsParrying())
            {
                animator?.SetBool("isAttacking", false);
                ShowWarning(false);
                ResetSpriteColor();
                StartCoroutine(ParryStunRoutine());
                yield break;
            }
            yield return null;
        }

        ShowWarning(false);
        ResetSpriteColor();

        if (state == State.Stunned || state == State.Dead)
            yield break;

        // ACTIVE HIT WINDOW
        state = State.Attacking;

        float activeTimer = 0f;
        bool hitLanded = false;

        while (activeTimer < attackActiveDuration && !hitLanded)
        {
            Collider2D[] hits = AttackUtilities.DetectEnemies(
                attackPoint.position, attackHitboxRange, playerLayer);

            foreach (Collider2D hit in hits)
            {
                if (playerInputActions != null && playerInputActions.IsParrying())
                {
                    animator?.SetBool("isAttacking", false);
                    StartCoroutine(ParryStunRoutine());
                    yield break;
                }

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

    private void ShowWarning(bool show)
    {
        if (warningIndicator != null)
            warningIndicator.SetActive(show);
    }

    private IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;

        float timer = 0f;
        while (state == State.WindingUp)
        {
            float t = (Mathf.Sin(timer * flashSpeed) + 1f) / 2f;
            spriteRenderer.color = Color.Lerp(originalColor, windupColor, t);
            timer += Time.deltaTime;
            yield return null;
        }
        ResetSpriteColor();
    }

    private void ResetSpriteColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private IEnumerator ParryStunRoutine()
    {
        state = State.Stunned;
        animator?.SetBool("isStunned", true);
        animator?.SetBool("isAttacking", false);

        Debug.Log($"{name} was parried!");
        TakeDamage(parryCounterDamage);

        yield return new WaitForSeconds(parryStunDuration);

        if (state == State.Dead) yield break;

        animator?.SetBool("isStunned", false);
        state = State.Chasing;
    }

    public void TakeDamage(int amount)
    {
        if (state == State.Dead) return;
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0) Die();
    }

    public void RegenHealth(int amount) =>
        currentHealth = Mathf.Min(currentHealth + Mathf.Max(0, amount), maxHealth);

    public int GetHealth() => currentHealth;
    public void Kill() => Die();

    private void Die()
    {
        state = State.Dead;
        StopAllCoroutines();
        ShowWarning(false);
        ResetSpriteColor();

        animator?.SetBool("isWalking", false);
        animator?.SetBool("isAttacking", false);
        animator?.SetBool("isStunned", false);
        animator?.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        if (interactablePrefab != null)
        {
            Vector3 lootPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(interactablePrefab, lootPos, Quaternion.identity);
        }

        Destroy(gameObject, 3f);
    }

    public void Interact()
    {
        switch (state)
        {
            case State.Dead:
                Debug.Log($"Looting {name}...");
                break;
            case State.Stunned:
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

        // Stop distance — enemy plants its feet here
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, meleeStartRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitboxRange);
        }
    }
}