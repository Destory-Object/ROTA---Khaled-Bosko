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

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        ReadPlayerInputs();
        CheckGrounded();
        HandleCoyoteTime();

        if (moveVector.x > 0.01f)
        {
            spriteRenderer.flipX = false;
            Debug.Log("Right");
        }
        else if (moveVector.x < -0.01f)
        {
            spriteRenderer.flipX = true;
            Debug.Log("Left");
        }
    }
    private void FixedUpdate()
    {if(isDashing)                       
        {
            playerRb.linearVelocityX = moveVector.x * dashSpeed;
        }   else
        {
            playerRb.linearVelocityX = moveVector.x * moveSpeed;
        }
    }
    void ReadPlayerInputs()
    {
        moveVector = moveAction.ReadValue<Vector2>();

        if(jumpAction.WasPerformedThisFrame() && isGrounded)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (dashAction.WasPerformedThisFrame())
        {
            isDashing = true;
        }
        if(dashAction.WasReleasedThisFrame())
        {
            isDashing = false;
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