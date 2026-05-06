using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShotgun : MonoBehaviour
{
    [Header("Shotgun")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int pelletsPerShot = 5;
    [SerializeField] private float spreadAngle = 30f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private int damagePerPellet = 1;
    [SerializeField] private float fireRate = 0.8f;

    [Header("Feedback")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float knockbackForce = 5f;

    private InputAction attackAction;
    private PlayerController pc;
    private Rigidbody2D rb;
    private float lastFireTime = -99f;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        if (attackAction.WasPressedThisFrame() && pc.playerState == "Normal")
        {
            if (Time.time >= lastFireTime + fireRate)
            {
                Shoot();
                lastFireTime = Time.time;
            }
        }
    }

    private void Shoot()
    {
        float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;
        Vector2 baseDirection = new Vector2(facing, 0f);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            // Spread pellets evenly across the spread angle
            float t = pelletsPerShot == 1 ? 0.5f : (float)i / (pelletsPerShot - 1);
            float angle = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, t);

            Vector2 direction = RotateVector(baseDirection, angle);

            GameObject pellet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
            if (pelletRb != null)
                pelletRb.linearVelocity = direction * projectileSpeed;

            Projectile projectileComp = pellet.GetComponent<Projectile>();
            if (projectileComp != null)
                projectileComp.damageAmount = damagePerPellet;
        }

        // Knockback pushes player back on shot
        rb.AddForce(new Vector2(-facing * knockbackForce, 0f), ForceMode2D.Impulse);

        StartCoroutine(HitStop());
    }

    private Vector2 RotateVector(Vector2 v, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad));
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;
    }
}