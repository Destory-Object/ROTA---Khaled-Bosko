using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPogo : MonoBehaviour
{
    public Transform pogoAttackPoint;
    public float pogoAttackRange = 0.5f;
    public LayerMask enemies;
    [SerializeField] int damageAmount;
    private float bounceForce = 8f;

    private Rigidbody2D playerRb;
    private InputAction PogoAttack;

    // Offsets
    public float attackDistance = 1f; // distance from player
    private Vector2 attackOffsetFront = new Vector2(1f, 0f); // in front
    private Vector2 attackOffsetUnder = new Vector2(0f, -1f); // under

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        PogoAttack = InputSystem.actions.FindAction("PogoAttack");
    }

    private void Update()
    {
        // Default to front
        pogoAttackPoint.localPosition = attackOffsetFront * attackDistance;

        // Switch to under if falling
        if (playerRb.linearVelocity.y < -0.1f)
        {
            pogoAttackPoint.localPosition = attackOffsetUnder * attackDistance;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with" + collision.gameObject.name);
        if (((1 << collision.gameObject.layer) & enemies) != 0)
        {
            Debug.Log("Enemy layer detected");
            if (playerRb.linearVelocity.y <= 0)
            {
                Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(pogoAttackPoint.position, pogoAttackRange, enemies);
                Debug.Log("Hit enemies count:" + hitEnemies.Length);
                foreach (Collider2D enemy in hitEnemies)
                {
                    Debug.Log("pogo hit" + enemy.name);
                    var enemyPatrol = enemy.GetComponent<EnemyPatrol>();
                    if (enemyPatrol != null)
                    {
                        enemyPatrol.TakeDamage(damageAmount);
                    }
                }
            }

            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            Debug.Log("bounce Applied");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pogoAttackPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pogoAttackPoint.position, pogoAttackRange);
    }
}