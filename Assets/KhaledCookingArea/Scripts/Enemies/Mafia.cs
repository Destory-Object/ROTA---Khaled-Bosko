using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Mafia : EnemyClass
{
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

    private GameObject alive;

    private Vector2 movement;

    private bool
        groundDetected,
        wallDetected;

    private void Start()
    {
        alive = transform.GetChild(0).gameObject;
        facingDirection = 1;

        currentState = State.Idle;
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
        currentState = State.None;

        yield return new WaitForSeconds(4);

        currentState = State.Walking;
    }

    IEnumerator WalkIdleLOOP()
    {
        currentState = State.Walking;

        yield return new WaitForSeconds(10);

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
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion
}
