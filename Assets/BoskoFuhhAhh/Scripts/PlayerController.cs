using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    [SerializeField] float jumpForce;
   
    [Header("Grounded Info")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundedLayers;
    bool isGrounded;


    [Header("Coyote Info")]
    [SerializeField] float CoyoteTime = 0.2f;
    private float coyoteTimer = 0.0f;

    [Header("Dash Info")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    // bool isDashing = false;


    [Header("Input Actions")]
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    Vector2 moveVector;

    Rigidbody2D playerRb;

    private SpriteRenderer spriteRenderer;

    [SerializeField] public string playerState;

    private PlayerInputActions inputActions;

    void Start()
    {
        inputActions = GetComponent<PlayerInputActions>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerState = "Normal";
    }
    private void Update()
    {
       // ReadPlayerInputs();
        CheckGrounded();
        HandleCoyoteTime();

        switch (playerState)
        {
            case "Normal":
         
                ReadPlayerInputs();
                break;

            case "Dashing":
                ReadPlayerInputs();
                break;

            case "attacing":
                ReadPlayerInputs();
                break;

            case "parryState":
                ReadPlayerInputs();

                break;
        }

        if (playerState == "parryState" || inputActions.IsParrying())
        {
            // disable movement or do specific parry logic
            GetComponent<PlayerDamageReceiver>().SwitchDamageRecieve(false);
            return;
        }
        else
        {
            GetComponent<PlayerDamageReceiver>().SwitchDamageRecieve(true);
        }
    }
    private void FixedUpdate() {

        if (playerState == "parryState" || inputActions.IsParrying())
        {
            // Do not move or handle movement during parry
            GetComponent<PlayerDamageReceiver>().SwitchDamageRecieve(false);
            return;
        }else
        {
            GetComponent<PlayerDamageReceiver>().SwitchDamageRecieve(true);
        }


        if (moveVector.x > 0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //Debug.Log("Right");
        }
        else if (moveVector.x < -0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            //Debug.Log("Left");
        }

        switch (playerState)
        {
            case "Normal":
                NormalMovement();
                break;

            case "Dashing":
                PlayerIsDashing();
                break;

            case "attacing":
                break;

            case "parryState":
                NormalMovement();

                break;
        }
    }
    void NormalMovement()
    {
        //Vector2 currentVelocity = playerRb.linearVelocity;
        //currentVelocity.x = moveVector.x * moveSpeed;
        //playerRb.linearVelocity = currentVelocity;
        
        
        playerRb.linearVelocityX = moveVector.x * moveSpeed;
    }
    void PlayerIsDashing()
    {

        //Vector2 currentVelocity = playerRb.linearVelocity;
        //currentVelocity.x = moveVector.x * moveSpeed;
        //playerRb.linearVelocity = currentVelocity;

       playerRb.linearVelocityX = moveVector.x * dashSpeed;
    }
    void ReadPlayerInputs()
    {
        if (playerState == "parryState" || inputActions.IsParrying())
            return;

        moveVector = moveAction.ReadValue<Vector2>().normalized;

        if(jumpAction.WasPerformedThisFrame() && isGrounded && playerState == "Normal")
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        } else if(jumpAction.WasPerformedThisFrame() && !isGrounded && coyoteTimer > 0)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }


        if (dashAction.WasPerformedThisFrame() && isGrounded)
        {
            playerState = "Dashing";
        }
        if(dashAction.WasReleasedThisFrame())
        {
            playerState = "Normal";
        }
    }
    private void HandleCoyoteTime()
    {
        if (isGrounded)
        { 
            coyoteTimer = CoyoteTime;
        }
        else
        {
            if (coyoteTimer > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
        }
    } 
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundedLayers);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    }
}