using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDagger : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.4f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float attackCooldown = 0.25f;

    [Header("Trail")]
    [SerializeField] private TrailRenderer attackTrail;

    private PlayerController pc;
    private WeaponManager weaponManager;
    private float lastAttackTime = -99f;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        weaponManager = GetComponent<WeaponManager>();
        if (attackTrail != null)
            attackTrail.emitting = false;
    }

    private void OnDisable()
    {
        if (attackTrail != null)
            attackTrail.emitting = false;
    }

    private void Update()
    {
        if (pc.playerState != "Normal") return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        InputAction slotAction = GetSlotAction();
        if (slotAction != null && slotAction.WasPressedThisFrame())
            Attack();
    }

    private InputAction GetSlotAction()
    {
        if (weaponManager.slotOne == WeaponType.Dagger)
            return InputSystem.actions.FindAction("SlotOne");
        if (weaponManager.slotTwo == WeaponType.Dagger)
            return InputSystem.actions.FindAction("SlotTwo");
        return null;
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
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

    private IEnumerator ShowTrail()
    {
        if (attackTrail != null)
        {
            attackTrail.emitting = true;
            yield return new WaitForSeconds(0.08f);
            attackTrail.emitting = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}