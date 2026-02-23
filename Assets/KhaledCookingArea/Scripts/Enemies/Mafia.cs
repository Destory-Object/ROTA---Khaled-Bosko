using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Mafia : EnemyClass , IHealth
{
    int health;
    [SerializeField] public Rigidbody2D aliveRb;

    [SerializeField]
    private float
        groundcheckDistance,
        wallcheckDistance,
        knockbackDuration;

    [SerializeField]
    private Transform
        groundcheck,
        wallcheck;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    float
        KnockbackStartTime;

    private int
        facingDirection,
        damageDirection;

    private Vector2 movement;

    private bool
        groundDetected,
        wallDetected,
        detectedPlayerOnce = false;

    [SerializeField] bool isActionable;


    [Header("Detection Position")]
    [SerializeField] Transform DetectPointFront;
    [SerializeField] float DPFradius;
    [SerializeField] bool detectedPlayer;

    [Header("Detection Position")]
    [SerializeField] Transform DetectPointBack;
    [SerializeField] float DPBradius;
    [SerializeField] bool backDetected;

    private void Start()
    {
        aliveRb = GetComponent<Rigidbody2D>();
        facingDirection = 1;

        currentState = State.Idle;
    }


    private void Detection()
    {
        detectedPlayer = Physics2D.OverlapCircle(DetectPointFront.position, DPFradius, whatIsPlayer);
        backDetected = Physics2D.OverlapCircle(DetectPointBack.position, DPBradius, whatIsPlayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(DetectPointFront.position, DPFradius);
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(DetectPointBack.position, DPBradius);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag(""))
    //    {
    //        TakeDmg();
    //    }

    //}

    IEnumerator GainBack()
    {
        isActionable = false;

        yield return new WaitForSeconds(1);

        isActionable = true;
        detectedPlayerOnce = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void Dash()
    {
        aliveRb.AddForce(transform.right * 200, ForceMode2D.Impulse);
    }
    void BackOff()
    {
        aliveRb.AddForce(-transform.right * 200, ForceMode2D.Impulse);
    }

    private void Update()
    {

        Detection();
        switch (currentState)
        {
            case State.Idle:
                StartCoroutine(IdleThinking());
                break;

                //case State.Dead:
                //    // UpdateDeadState();
                //    break;
        }

        if (backDetected == true)
        {
            Flip();
            //  StartCoroutine(FlipBehavior());
        }

        if (detectedPlayer == true)
        {
            if (!detectedPlayerOnce)
            {
                detectedPlayerOnce = true;
                Debug.Log("sees player");
                int value = Random.Range(1, 3);

                if (value == 1)
                {
                    Debug.Log("im doing some"); //DO SOMETHING
                    if (isActionable == true)
                    {
                        Dash();
                        StartCoroutine(GainBack());
                    }
                }
                else
                {
                    Debug.Log("im doing some 2"); //DO SOMETHING ELSE
                    if (isActionable == true)
                    {
                        BackOff();
                        StartCoroutine(GainBack());
                    }
                }
            }
        }

    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Walking:
                Movement();
                StartCoroutine(WalkIdleLOOP());
                break;
        }
    }

    IEnumerator IdleThinking()
    {

        yield return new WaitForSeconds(4);

        currentState = State.Walking;
    }

    IEnumerator WalkIdleLOOP()
    {


        yield return new WaitForSeconds(5);

        currentState = State.Idle;
    }

    #region MoveFunction

    public override void Movement()
    {
        groundDetected = Physics2D.Raycast(groundcheck.position, Vector2.down, groundcheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallcheck.position, transform.right, wallcheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            Debug.Log("går");
            movement.Set(movementSpeed * facingDirection, aliveRb.linearVelocityY);
            aliveRb.linearVelocity = movement;


        }
    }



    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public void RegenHealth(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public int GetHealth()
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
