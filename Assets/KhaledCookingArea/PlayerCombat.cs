using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Attack();
            Debug.Log("dennis");
        }
    }

    void Attack()
    {

       Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position,attackRange,enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit" +  enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(damageAmount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
