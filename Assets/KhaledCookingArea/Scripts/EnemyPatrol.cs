using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{

    #region EnemyHealthSystem
    public int maxHealth;
    public int currentHealth;


    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        //take damage anim

        if (currentHealth <= 0)
        {
            vSpeed = 0;
            hSpeed = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
               
        //Destroy(gameObject);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyPatrol>().enabled = false;
        //gameObject.SetActive(false);
        //die anim
        //disable the enemy

    }
    #endregion

    #region Behaviour
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;

    private Transform currentPoint;
    public float vSpeed;
    public float hSpeed;

    [SerializeField] bool horizontalMovement;
    [SerializeField] bool verticalMovement;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
        if(currentPoint == pointB.transform)
        {
            rb.linearVelocity = new Vector2(vSpeed, hSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(-vSpeed, -hSpeed);
        }

        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            flip();
            currentPoint = pointA.transform;
        } 
        
        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            flip();
            currentPoint = pointB.transform;
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        
        if(verticalMovement == true)
        {
        localScale.x *= -1;
        }
        
        if(horizontalMovement == true)
        {
        localScale.y *= -1;
        }

        transform.localScale = localScale;
    }
    #endregion

    
}
