using UnityEngine;

public class EnemyShooter : MonoBehaviour, IHealth
{
    public GameObject projectilePrefab; // Assign in inspector
    public Transform firePoint; // Point from where projectiles are fired
    public float shootInterval = 2f; // Time between shots
    public float projectileSpeed = 5f;
    public int maxHealth = 3;

    public Animator animator; // Assign in inspector or get in Start()
    public string shootAnimationTrigger = "Shoot"; // Trigger name in Animator

    private int currentHealth;
    private Transform playerTransform;
    private float shootTimer;


    public GameObject interactablePrefab; // Assign in inspector
    public Transform spawnPoint; // Optional: assign in inspector or use enemy position

    private void Start()
    {
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (playerTransform == null)
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
        {
            animator.SetTrigger(shootAnimationTrigger);
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Optional: set projectile damage if needed
        Projectile projectileComp = projectile.GetComponent<Projectile>();
        if (projectileComp != null)
        {
            projectileComp.damageAmount = 1; // Or any damage value
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void RegenHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void Kill()
    {
        Die();
    }

    private void Die()
    {
        // Spawn the interactable object at a specific point (e.g., enemy position)
        if (interactablePrefab != null && spawnPoint != null)
        {
            Instantiate(interactablePrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (interactablePrefab != null)
        {
            // If spawnPoint not set, spawn at the enemy's current position
            Instantiate(interactablePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}