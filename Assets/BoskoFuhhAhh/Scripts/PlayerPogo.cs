using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPogo : MonoBehaviour
{
    public Transform pogoAttackPoint;
    public float pogoAttackRange = 0.5f;
    public LayerMask enemies;
    [SerializeField] int damageAmount;
    [SerializeField] float bounceForce = 8f;

    private Rigidbody2D playerRb;
    private InputAction PogoAttack;

    private PlayerController pc;

    // Offsets
    public float attackDistance = 1f;
    private Vector2 attackOffsetFront = new Vector2(1f, 0f); // in front
    private Vector2 attackOffsetUnder = new Vector2(0f, -1f); // under

    Animator anim;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        //  anim = GetComponent<Animator>();
    }
    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        PogoAttack = InputSystem.actions.FindAction("PogoAttack");
    }

    private void Update()
    {

        pogoAttackPoint.localPosition = attackOffsetFront * attackDistance;


        if (playerRb.linearVelocity.y < -0.1f)
        {
            pogoAttackPoint.localPosition = attackOffsetUnder * attackDistance;

            if (PogoAttack.WasPressedThisFrame())
                TestPogo();
        }
    }

    void TestPogo()
    {
        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(pogoAttackPoint.position, pogoAttackRange, enemies);    
        if (hitEnemies.Length > 0)
        {
            Debug.Log("Enemy layer detected");
            Debug.Log("Hit enemies count:" + hitEnemies.Length);
            foreach (Collider2D enemies in hitEnemies)
            {
                Debug.Log("Collision with" + enemies.gameObject.name);
                if (playerRb.linearVelocity.y <= 0)
                {
                    Debug.Log("pogo hit" + enemies.name);
                    //var enemyPatrol = enemies.GetComponent<EnemyPatrol>();

                    IHealth enemyHealth = enemies.GetComponent<IHealth>();

                    if (enemyHealth != null)
                    {
                        //anim = enemyPatrol.gameObject.GetComponent<Animator>();
                        enemyHealth.TakeDamage(damageAmount);
                        //anim.SetTrigger("Damage");  //Fixa här
                        //playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                        playerRb.AddForceY(bounceForce, ForceMode2D.Impulse);
                        Debug.Log("bounce Applied");
                    }
                }

                //playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                //Debug.Log("bounce Applied");
            }
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