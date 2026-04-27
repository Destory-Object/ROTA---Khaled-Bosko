using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour, IHealth, ILaunchable
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public int maxHealth = 3;
    public Animator animator;
    public string shootAnimationTrigger = "Shoot";

    [Header("Detection")]
    public float detectionRange = 7f;

    [Header("Keep Distance")]
    public float preferredRange = 5f;
    public float tooCloseRange = 3f;
    public float moveSpeed = 2f;

    private int currentHealth;
    private Transform playerTransform;
    private float shootTimer;
    private Rigidbody2D rb;
    private bool isLaunched = false;

    public GameObject interactablePrefab;
    public Transform spawnPoint;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerTransform == null || isLaunched) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        HandleMovement(dist);

        if (dist > detectionRange) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            PlayShootAnimation();
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
    }

    private void HandleMovement(float dist)
    {
        if (rb == null) return;

        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;

        if (dist < tooCloseRange)
        {
            rb.linearVelocity = new Vector2(awayFromPlayer.x * moveSpeed, rb.linearVelocity.y);
            FlipToFacePlayer();
        }
        else if (dist > preferredRange && dist <= detectionRange)
        {
            rb.linearVelocity = new Vector2(-awayFromPlayer.x * moveSpeed, rb.linearVelocity.y);
            FlipToFacePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void FlipToFacePlayer()
    {
        if (playerTransform.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    //ILaunchable
    public void Launch(Vector2 force)
    {
        isLaunched = true;

        //Must be Dynamic before applying force
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = force;

        animator?.SetTrigger("isLaunched");

        Debug.Log($"{name} launched! Velocity: {rb.linearVelocity}");

        StartCoroutine(LandRoutine());
    }

    private IEnumerator LandRoutine()
    {
        //Wait until moving upward is done then wait to land
        yield return new WaitForSeconds(0.3f);

        while (rb.linearVelocity.y > -0.1f || IsAirborne())
            yield return null;

        //Landed — re-freeze horizontal so player can't push it again
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isLaunched = false;

        Debug.Log($"{name} landed.");
    }

    private bool IsAirborne()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, Vector2.down, 0.6f,
            ~LayerMask.GetMask("Enemies", "Player"));
        return hit.collider == null;
    }

    private void PlayShootAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(shootAnimationTrigger))
            animator.SetTrigger(shootAnimationTrigger);
    }

    private void ShootAtPlayer()
    {
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
            projectileRb.linearVelocity = direction * projectileSpeed;

        Projectile projectileComp = projectile.GetComponent<Projectile>();
        if (projectileComp != null)
            projectileComp.damageAmount = 1;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public int GetHealth() => currentHealth;
    public void RegenHealth(int amount) =>
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    public void Kill() => Die();

    private void Die()
    {
        if (interactablePrefab != null && spawnPoint != null)
            Instantiate(interactablePrefab, spawnPoint.position, Quaternion.identity);
        else if (interactablePrefab != null)
            Instantiate(interactablePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, preferredRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);
    }
}