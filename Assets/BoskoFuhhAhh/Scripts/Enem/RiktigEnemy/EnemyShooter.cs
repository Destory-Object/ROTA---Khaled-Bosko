using UnityEngine;

public class EnemyShooter : MonoBehaviour, IHealth
{
    public int contactDamage = 1;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public int maxHealth = 3;
    public Animator animator;
    public string shootAnimationTrigger = "Shoot";

    [Header("Detection")]
    public float detectionRange = 7f;   // Enemy only shoots when player is within this distance

    private int currentHealth;
    private Transform playerTransform;
    private float shootTimer;
    public GameObject interactablePrefab;
    public Transform spawnPoint;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeAll;
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        //Only shoot if player is within detection range
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist > detectionRange)
            return;
 

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            PlayShootAnimation();
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
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
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;

        Projectile projectileComp = projectile.GetComponent<Projectile>();
        if (projectileComp != null)
            projectileComp.damageAmount = 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            IHealth health = collision.collider.GetComponent<IHealth>();
            if (health != null)
                health.TakeDamage(contactDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
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
    }
}