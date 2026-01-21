using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Mafia : EnemyClass
{
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
        wallDetected;

    private void Start()
    {
        aliveRb = GetComponent<Rigidbody2D>();
        facingDirection = 1;

        currentState = State.Idle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            Debug.Log("sees player");
            int value = Random.Range(1, 3);

            if (value == 1)
            {
                Debug.Log("im doing some"); //DO SOMETHING
            }
            else
            {
                Debug.Log("im doing some 2"); //DO SOMETHING ELSE
            }
        }
      
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                StartCoroutine(IdleThinking());
                break;

                //case State.Dead:
                //    // UpdateDeadState();
                //    break;
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

            // Debug.Log(facingDirection);
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
       transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion
}
