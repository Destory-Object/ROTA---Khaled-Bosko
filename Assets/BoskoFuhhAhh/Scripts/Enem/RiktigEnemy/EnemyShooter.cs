using UnityEngine;

public class EnemyShooter : MonoBehaviour, IHealth
{
    public GameObject projectilePrefab; // Assign in inspector
    public Transform firePoint; // Point from where projectiles are fired
    public float shootInterval = 2f; // Time between shots
    public float projectileSpeed = 5f;
    public int maxHealth = 3;

    private int currentHealth;
    private Transform playerTransform;
    private float shootTimer;

    private void Start()
    {
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;
    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootInterval;
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
        // Play death animation or effects here
        Destroy(gameObject);
    }
}