using UnityEngine;

public enum WeaponType { Sword, Shotgun }

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private PlayerInputActions sword;
    [SerializeField] private PlayerShotgun shotgun;

    [Header("Drop")]
    [SerializeField] private GameObject swordPickupPrefab;
    [SerializeField] private GameObject shotgunPickupPrefab;

    public WeaponType currentWeapon = WeaponType.Sword;

    private void Start()
    {
        EquipWeapon(currentWeapon, dropCurrent: false);
    }

    public void PickUp(WeaponType newWeapon)
    {
        if (newWeapon == currentWeapon) return;

        DropCurrentWeapon();
        EquipWeapon(newWeapon, dropCurrent: false);
    }

    private void DropCurrentWeapon()
    {
        GameObject prefabToDrop = currentWeapon == WeaponType.Sword
            ? swordPickupPrefab
            : shotgunPickupPrefab;

        if (prefabToDrop != null)
            Instantiate(prefabToDrop, transform.position, Quaternion.identity);
    }

    private void EquipWeapon(WeaponType weapon, bool dropCurrent)
    {
        currentWeapon = weapon;

        sword.enabled = weapon == WeaponType.Sword;
        shotgun.enabled = weapon == WeaponType.Shotgun;

        Debug.Log($"Equipped: {weapon}");
    }
}