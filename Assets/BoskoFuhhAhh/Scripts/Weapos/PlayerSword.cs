// CHANGES: added AudioManager.Play calls for swing + hit.
using System.Collections;
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
    [SerializeField] private SwordSlashEffect slashEffect;
    private PlayerController pc;
    private WeaponManager weaponManager;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        weaponManager = GetComponent<WeaponManager>();
        if (attackTrail != null)
            attackTrail.emitting = false;
    }
    private void Update()
    {
        if (pc.playerState != "Normal") return;
        InputAction slotAction = GetSlotAction();
        if (slotAction != null && slotAction.WasPressedThisFrame())
            Attack();
    }
    private InputAction GetSlotAction()
    {
        if (weaponManager.slotOne == WeaponType.Sword)
            return InputSystem.actions.FindAction("SlotOne");
        if (weaponManager.slotTwo == WeaponType.Sword)
            return InputSystem.actions.FindAction("SlotTwo");
        return null;
    }
    private void Attack()
    {
        slashEffect?.PlaySlash();
        StartCoroutine(ShowTrail());
        AudioManager.Play("SwordSwing");
        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(
            attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            IHealth healthComp = enemy.GetComponent<IHealth>();
            if (healthComp != null)
            {
                CombatEffects.DealDamage(healthComp, damageAmount, enemy.transform.position);
                AudioManager.Play("HitImpact");
            }
        }
    }
    private IEnumerator ShowTrail()
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