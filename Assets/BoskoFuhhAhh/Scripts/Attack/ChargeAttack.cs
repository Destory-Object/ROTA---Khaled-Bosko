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

    [Header("Crit")]
    [SerializeField] private float critChance = 0.25f;          
    [SerializeField] private float critMultiplier = 2f;        

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
        //Only active while player is dashing
        if (pc.playerState != "Dashing")
        {
            hitLanded = false;  //Reset so the next dash can hit again
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

            //crit roll
            bool isCrit = Random.value <= critChance;
            int damage = isCrit
                ? Mathf.RoundToInt(chargeDamage * critMultiplier)
                : chargeDamage;

            health.TakeDamage(damage);

            //Launch upward
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

            Debug.Log($"Charge hit! Crit: {isCrit} | Damage: {damage}");

            //hit stoop
            StartCoroutine(HitStop());

            hitLanded = true;
            lastChargeTime = Time.time;

            //End dash right after connecting so player doesnt fly past
            pc.playerState = "Normal";
            break;
        }
    }

    //Freezes time briefly on hit — makes the impact feel heavy
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