// CHANGES: replaced local Time.timeScale coroutine with HitStopManager.RequestHitStop (now a serialized field instead of the hardcoded 0.05f), added AudioManager.Play("ShotgunFire").
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShotgun : MonoBehaviour
{
    [Header("Shotgun")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int pelletsPerShot = 4;
    [SerializeField] private float spreadAngle = 25f;
    [SerializeField] private float projectileSpeed = 24f;
    [SerializeField] private int damagePerPellet = 2;
    [SerializeField] private float fireRate = 0.8f;

    [Header("Feedback")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float hitStopDuration = 0.05f;

    private PlayerController pc;
    private WeaponManager weaponManager;
    private Rigidbody2D rb;
    private float lastFireTime = -99f;

    private Dictionary<IHealth, int> pendingDamage = new Dictionary<IHealth, int>();
    private Dictionary<IHealth, Vector3> pendingPositions = new Dictionary<IHealth, Vector3>();
    private bool collectingHits = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        weaponManager = GetComponent<WeaponManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (pc.playerState != "Normal") return;
        if (Time.time < lastFireTime + fireRate) return;

        InputAction slotAction = GetSlotAction();
        if (slotAction != null && slotAction.WasPressedThisFrame())
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    private InputAction GetSlotAction()
    {
        if (weaponManager.slotOne == WeaponType.Shotgun)
            return InputSystem.actions.FindAction("SlotOne");
        if (weaponManager.slotTwo == WeaponType.Shotgun)
            return InputSystem.actions.FindAction("SlotTwo");
        return null;
    }

    public void RegisterHit(IHealth target, int damage, Vector3 position)
    {
        if (pendingDamage.ContainsKey(target))
            pendingDamage[target] += damage;
        else
        {
            pendingDamage[target] = damage;
            pendingPositions[target] = position;
        }

        if (!collectingHits)
            StartCoroutine(FlushHits());
    }

    private IEnumerator FlushHits()
    {
        collectingHits = true;
        yield return new WaitForSeconds(0.15f);

        foreach (var kvp in pendingDamage)
        {
            if (kvp.Key != null)
                CombatEffects.DealDamage(kvp.Key, kvp.Value, pendingPositions[kvp.Key]);
        }

        pendingDamage.Clear();
        pendingPositions.Clear();
        collectingHits = false;
    }

    private void Shoot()
    {
        float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;
        Vector2 baseDirection = new Vector2(facing, 0f);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            float t = pelletsPerShot == 1 ? 0.5f : (float)i / (pelletsPerShot - 1);
            float angle = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, t);
            Vector2 direction = RotateVector(baseDirection, angle);

            GameObject pellet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
            if (pelletRb != null)
                pelletRb.linearVelocity = direction * projectileSpeed;

            PlayerProjectile projectileComp = pellet.GetComponent<PlayerProjectile>();
            if (projectileComp != null)
            {
                projectileComp.damageAmount = damagePerPellet;
                projectileComp.shotgun = this;
            }
        }

        AudioManager.Play("ShotgunFire");
        rb.AddForce(new Vector2(-facing * knockbackForce, 0f), ForceMode2D.Impulse);
        HitStopManager.RequestHitStop(hitStopDuration);
    }

    private Vector2 RotateVector(Vector2 v, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad));
    }
}