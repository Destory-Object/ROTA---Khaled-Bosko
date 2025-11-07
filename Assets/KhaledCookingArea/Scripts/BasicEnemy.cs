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
        wallcheckDistance;
    [SerializeField]
    private Transform
        groundcheck,
        wallcheck;
    [SerializeField]
    private LayerMask WhatIsGround;

    private bool
        groundDetected,
        wallDetected;

    private void Update()
    {
        switch (currentState) 
        {
            case State.Walking:
                UpdateWalkingState();
                break;

            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
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
            //flip

        }
    }

    private void ExitWalkingState()
    {

    }
    
    //---KnockbackState------------------------------------------------------------------------

    private void EnterKnockbackState()
    {

    }
    
    private void UpdateKnockbackState()
    {

    }

    private void ExitKnockbackState()
    {

    }
    
    //---DeadState------------------------------------------------------------------------

    private void EnterDeadState()
    {

    }
    
    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //---OtherFunctions------------------------------------------------------------------------

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

}
