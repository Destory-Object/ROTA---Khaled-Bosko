using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] private int damageAmount;

    [Header("Trail")]
    [SerializeField] private TrailRenderer attackTrail;

    private InputAction attackAction;
    private PlayerController pc;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        attackAction = InputSystem.actions.FindAction("Attack");

        if (attackTrail != null)
            attackTrail.emitting = false;
    }

    private void OnEnable()
    {
        attackAction.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        attackAction.performed -= OnAttackPerformed;

        if (attackTrail != null)
            attackTrail.emitting = false;
    }

    private void OnAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (pc.playerState == "Normal")
            Attack();
    }

    private void Attack()
    {
        StartCoroutine(ShowTrail());

        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(
            attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IHealth healthComp = enemy.GetComponent<IHealth>();
            if (healthComp != null)
                CombatEffects.DealDamage(healthComp, damageAmount, enemy.transform.position);
        }
    }

    private System.Collections.IEnumerator ShowTrail()
    {
        if (attackTrail != null)
        {
            attackTrail.emitting = true;
            yield return new WaitForSeconds(0.15f);
            attackTrail.emitting = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}