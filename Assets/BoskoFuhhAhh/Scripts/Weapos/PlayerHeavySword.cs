using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHeavySword : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.9f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int damageAmount = 3;
    [SerializeField] private float attackCooldown = 0.7f;

    [Header("Hit Stop")]
    [SerializeField] private float hitStopDuration = 0.1f;

    [Header("Trail")]
    [SerializeField] private TrailRenderer attackTrail;
    [SerializeField] private SwordSlashEffect slashEffect;

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
        if (weaponManager.slotOne == WeaponType.HeavySword)
            return InputSystem.actions.FindAction("SlotOne");
        if (weaponManager.slotTwo == WeaponType.HeavySword)
            return InputSystem.actions.FindAction("SlotTwo");
        return null;
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        slashEffect?.PlaySlash();
        StartCoroutine(ShowTrail());

        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(
            attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IHealth healthComp = enemy.GetComponent<IHealth>();
            if (healthComp != null)
            {
                CombatEffects.DealDamage(healthComp, damageAmount, enemy.transform.position);
                StartCoroutine(HitStop());
            }
        }
    }

    private IEnumerator ShowTrail()
    {
        if (attackTrail != null)
        {
            attackTrail.emitting = true;
            yield return new WaitForSeconds(0.25f);
            attackTrail.emitting = false;
        }
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}