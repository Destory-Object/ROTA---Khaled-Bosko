using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    [SerializeField] float jumpForce = 7f;
   
    [Header("Grounded Info")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundedLayers;
    bool isGrounded;


    [Header("Coyote Info")]
    [SerializeField] float CoyoteTime = 0.2f;
    private float coyoteTimer = 0.0f;
    
    [Header("Dash Info")]
    [SerializeField] float dashSpeed = 20.0f;
    [SerializeField] float dashDuration = 0.5f;
     bool isDashing = false;


    [Header("Input Actions")]
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    Vector2 moveVector;

    Rigidbody2D playerRb;

    private SpriteRenderer spriteRenderer;

    [SerializeField] string playerState;

    void Start()
    {
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
         
                ReadPlayerInputs();
                break;

            case "Dashing":
                ReadPlayerInputs();
                break;

            case "attacing":
                ReadPlayerInputs();
                break;

        }





    }
    private void FixedUpdate() {



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

        }

    }

    void NormalMovement()
    {
        playerRb.linearVelocityX = moveVector.x * moveSpeed;
    }
    void PlayerIsDashing()
    {
        playerRb.linearVelocityX = moveVector.x * dashSpeed;
    }
    void ReadPlayerInputs()
    {
        moveVector = moveAction.ReadValue<Vector2>();

        if(jumpAction.WasPerformedThisFrame() && isGrounded && playerState == "Normal")
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }


        if (dashAction.WasPerformedThisFrame())
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