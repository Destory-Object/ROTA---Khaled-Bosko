using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPogo : MonoBehaviour
{

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;
    private float bounceForce = 8f;

    InputAction  PogoAttack;
    private Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with"+ collision.gameObject.name);
         if (((1 << collision.gameObject.layer) & enemyLayers) != 0)
        {
            Debug.Log("Enemy layer detected");
            if (playerRb.linearVelocity.y <= 0)
            {
                Collider2D[] hitEnemeies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                Debug.Log("Hit enemies count:"+ hitEnemeies.Length);
                foreach (Collider2D enemy in hitEnemeies)
                {
                    Debug.Log("pogo hit"+ enemy.name); ;
                    var enemyPatrol = enemy.GetComponent<EnemyPatrol>();
                    if(enemyPatrol != null)
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
        if (attackPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
