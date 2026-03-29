using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    public int damageAmount = 1;

    [Header("Parry Reflection")]
    [SerializeField] private float reflectSpeedMultiplier = 1.5f;
    [SerializeField] private float reflectHitRadius = 0.3f;     // Manual hit detection radius on reflected shot

    [Header("Parry Effects")]
    [SerializeField] private ParticleSystem parryParticles;
    [SerializeField] private Light2D parryLight;
    [SerializeField] private float lightFlashIntensity = 3f;
    [SerializeField] private float lightFadeSpeed = 8f;

    private bool isReflected = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Manual hit detection for reflected shot — bypasses layer matrix entirely
        if (!isReflected) return;

        // Find all "Enemy" tagged objects and check distance manually
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist <= reflectHitRadius)
            {
                Debug.Log("Reflected shot hit: " + enemy.name);
                IHealth health = enemy.GetComponent<IHealth>();
                if (health != null)
                    health.TakeDamage(damageAmount);

                StartCoroutine(ParryEffect(destroyAfter: true));
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Reflected shot movement is handled in Update — ignore all triggers
        if (isReflected)
        {
            if (collision.CompareTag("Environment"))
                Destroy(gameObject);
            return;
        }

        // ── Normal shot hitting the player ────────────────────────────────
        if (collision.CompareTag("Player"))
        {
            PlayerInputActions inputActions = collision.GetComponent<PlayerInputActions>();
            if (inputActions != null && inputActions.IsParrying())
            {
                Debug.Log("Projectile reflected!");
                ReflectAtEnemy();
                return;
            }

            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
                health.TakeDamage(damageAmount);

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    private void ReflectAtEnemy()
    {
        isReflected = true;

        // Aim at nearest enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform target = null;
        float closest = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = enemy.transform;
            }
        }

        Vector2 reflectDirection = target != null
            ? ((Vector2)target.position - (Vector2)transform.position).normalized
            : -rb.linearVelocity.normalized;

        rb.linearVelocity = reflectDirection * (rb.linearVelocity.magnitude * reflectSpeedMultiplier);

        // Flip sprite
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);

        StartCoroutine(ParryEffect(destroyAfter: false));
    }

    private IEnumerator ParryEffect(bool destroyAfter = true)
    {
        if (parryParticles != null)
        {
            parryParticles.transform.SetParent(null);
            parryParticles.Play();
            Destroy(parryParticles.gameObject, parryParticles.main.duration + 0.5f);
        }

        if (parryLight != null)
        {
            parryLight.transform.SetParent(null);
            parryLight.intensity = lightFlashIntensity;
            parryLight.enabled = true;

            while (parryLight.intensity > 0.05f)
            {
                parryLight.intensity = Mathf.Lerp(
                    parryLight.intensity, 0f, Time.deltaTime * lightFadeSpeed);
                yield return null;
            }

            Destroy(parryLight.gameObject);
        }

        if (destroyAfter)
            Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!isReflected) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, reflectHitRadius);
    }
}