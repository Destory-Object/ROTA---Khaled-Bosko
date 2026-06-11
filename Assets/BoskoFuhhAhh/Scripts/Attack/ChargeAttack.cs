using System.Collections;
using UnityEngine;

public class ChargeAttack : MonoBehaviour
{
    [Header("Charge Attack")]
    [SerializeField] private float chargeHitRadius = 1f;       
    [SerializeField] private int chargeDamage = 2;             
    [SerializeField] private LayerMask enemyLayer;              

    [Header("Launch")]
    [SerializeField] private float launchForceUp = 12f;         
    [SerializeField] private float launchForceBack = 4f;           

    [Header("Charge Feel")]
    [SerializeField] private float hitStopDuration = 0.08f;     
    [SerializeField] private float chargeCooldown = 1f;       

    private PlayerController pc;
    private float lastChargeTime = -99f;
    private bool hitLanded = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {

        if (pc.playerState != "Dashing")
        {
            hitLanded = false;
            return;
        }

        if (hitLanded) return;
        if (Time.time < lastChargeTime + chargeCooldown) return;

        CheckChargeHit();
    }

    private void CheckChargeHit()
    {
        Collider2D[] hits = AttackUtilities.DetectEnemies(
            transform.position, chargeHitRadius, enemyLayer);
        if (hits.Length == 0) return;

        foreach (Collider2D hit in hits)
        {
            IHealth health = hit.GetComponent<IHealth>();
            if (health == null) continue;

            // Let CombatEffects handle crit — remove the local roll
            CombatEffects.DealDamage(health, chargeDamage, hit.transform.position); // was: manual crit + health.TakeDamage

            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.bodyType = RigidbodyType2D.Dynamic;
                enemyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                float knockDir = hit.transform.position.x > transform.position.x ? 1f : -1f;
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.AddForce(
                    new Vector2(knockDir * launchForceBack, launchForceUp),
                    ForceMode2D.Impulse);
            }

            StartCoroutine(HitStop());
            hitLanded = true;
            lastChargeTime = Time.time;
            pc.playerState = "Normal";
            break;
        }
    }


    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chargeHitRadius);
    }
}