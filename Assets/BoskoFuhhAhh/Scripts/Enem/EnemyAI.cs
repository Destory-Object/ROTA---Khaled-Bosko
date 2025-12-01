using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;           
    public float moveSpeed = 3f;            
    public float attackRange = 2f;          
    public float attackCooldown = 1.5f;    
    public int damageAmount = 10;           

    public Animator animator;                
    public PlayerDamageReceiver playerDamageReceiver; 

    private float lastAttackTime;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            
            MoveTowardsPlayer();
        }
        else
        {
            
            Attack();
        }
    }

    void MoveTowardsPlayer()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
            animator.SetTrigger("Walk");
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("Attack");
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
           
            if (playerDamageReceiver != null)
            {
                playerDamageReceiver.TakeDamage(damageAmount);
            }
            Debug.Log("Enemy attacked!");
        }
    }
}