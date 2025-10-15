using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [seralizeField] public float moveSpeed = 5f;     
    [seralizeField] public float jumpForce = 7f;     

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
       
        float moveInput = Input.GetAxisRaw("Horizontal");

       
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
