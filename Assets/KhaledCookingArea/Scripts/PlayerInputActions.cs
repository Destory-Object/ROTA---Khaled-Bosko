using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;

    private InputAction attackAction;

    private PlayerInput playerInput;


    private void Awake()
    {
        
        attackAction = InputSystem.FindAction("Attack");

        if (attackAction != null)
        {
            attackAction.performed += ctx => Attack();
        }
        else
        {
            Debug.LogError("Attack action not found!");
        }
    }

    private void OnEnable()
    {
        attackAction?.Enable(); // ? betyder att den kan ha värdet null
    }

    private void OnDisable()
    {
        attackAction?.Disable();
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
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