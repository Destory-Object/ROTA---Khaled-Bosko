using System.Xml.Serialization;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    private enum State
    {
        Walking,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float
        groundcheckDistance,
        wallcheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration;

    [SerializeField]
    private Transform
        groundcheck,
        wallcheck;

    [SerializeField]
    private LayerMask WhatIsGround;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField] float 
        currentHealth,
        KnockbackStartTime;

    private int 
        facingDirection,
        damageDirection;

    private Vector2 movement;

    private bool
        groundDetected,
        wallDetected;

    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = GetComponentInChildren<Rigidbody2D>();
        aliveAnim = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState) 
        {
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Walking:
                UpdateWalkingState();
                break;

          
        }
    }

    //---WalkingState------------------------------------------------------------------------

    private void EnterWalkingState()
    {

    }
    
    private void UpdateWalkingState()
    {
        groundDetected = Physics2D.Raycast(groundcheck.position, Vector2.down, groundcheckDistance, WhatIsGround);
        wallDetected = Physics2D.Raycast(wallcheck.position, transform.right, wallcheckDistance, WhatIsGround);

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

    private void ExitWalkingState()
    {

    }
    
    //---KnockbackState------------------------------------------------------------------------

    private void EnterKnockbackState()
    {
        KnockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.linearVelocity = movement;
        aliveAnim.SetBool("Knockback", true);

        Debug.Log("entered knockback state");
    }
    
    private void UpdateKnockbackState()
    {
        if (Time.time > KnockbackStartTime + knockbackDuration)
        {
            Debug.Log("Should be walking");
            SwitchState(State.Walking);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }

    //---DeadState------------------------------------------------------------------------

    private void EnterDeadState()
    {
        //spwan chunks and blood 
        Destroy(gameObject);
    }
    
    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //---OtherFunctions------------------------------------------------------------------------

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        if( attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //HIT PARTIKEL SAKNAS LIL BRO (Gör parikelHanlder först, sen instantiate partikel) - Kl 07:15 Btw... 

        if(currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth < 0.0f)
        {
            SwitchState(State.Dead);
        }

    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Walking:
                ExitWalkingState();
                break;

            case State.Knockback:
                ExitKnockbackState();
                break;

            case State.Dead:
                ExitDeadState();
                break;
        }
        
        switch(state)
        {
            case State.Walking:
                EnterWalkingState();
                break;

            case State.Knockback:
                EnterKnockbackState();
                break;

            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundcheck.position, new Vector2(groundcheck.position.x, groundcheck.position.y - groundcheckDistance));
        Gizmos.DrawLine(wallcheck.position, new Vector2(wallcheck.position.x + wallcheckDistance, wallcheck.position.y));
        
    }

}
