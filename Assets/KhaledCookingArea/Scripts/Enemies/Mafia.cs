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
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Knockback:
               // UpdateKnockbackState();
                break;
            case State.Dead:
               // UpdateDeadState();
                break;
        }
    }

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
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}
