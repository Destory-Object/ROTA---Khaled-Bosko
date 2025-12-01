using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject attackZone; 
    public float attackDuration = 0.5f; 
    public float attackCooldown = 2f; 

    private bool isAttacking = false;

    private void Start()
    {
        if (attackZone != null)
            attackZone.SetActive(false);
        InvokeRepeating(nameof(PerformAttack), attackCooldown, attackCooldown);
    }

    private void PerformAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackZone.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackZone.SetActive(false);
        isAttacking = false;
    }
}