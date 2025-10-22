using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputActions : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;

    private InputAction attackAction;

  


    private void Awake()
    {

        attackAction = InputSystem.actions.FindAction("Attack");

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
            enemy.GetComponent<EnemyPatrol>().TakeDamage(damageAmount);
        }
    }

    private void Parrying()
    {
        //om du klickar F så kommer den köra Parry();
    }

    IEnumerator Parry()
    {

        FindAnyObjectByType<PlayerController>().playerState = "Parry";
        
        
        yield return new WaitForSeconds(0.3f);


        FindAnyObjectByType<PlayerController>().playerState = "Normal";

    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}