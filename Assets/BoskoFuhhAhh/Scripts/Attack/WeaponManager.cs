using UnityEngine;

public enum WeaponType { None, Sword, Shotgun }

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerSword sword;
    [SerializeField] private PlayerShotgun shotgun;

    [Header("Drop Prefabs")]
    [SerializeField] private GameObject swordPickupPrefab;
    [SerializeField] private GameObject shotgunPickupPrefab;

    public WeaponType currentWeapon = WeaponType.None;
    [SerializeField] private WeaponType startingWeapon = WeaponType.Sword;

    private void Start()
    {
        sword.enabled = false;
        shotgun.enabled = false;

        if (startingWeapon != WeaponType.None)
            EquipWeapon(startingWeapon);
    }

    public void PickUp(WeaponType newWeapon)
    {
        if (newWeapon == currentWeapon) return;

        DropCurrentWeapon();
        EquipWeapon(newWeapon);
    }

    private void DropCurrentWeapon()
    {
        GameObject prefabToDrop = currentWeapon == WeaponType.Sword
            ? swordPickupPrefab
            : currentWeapon == WeaponType.Shotgun
                ? shotgunPickupPrefab
                : null;

        if (prefabToDrop != null)
            Instantiate(prefabToDrop, transform.position + Vector3.right, Quaternion.identity);
    }

    private void EquipWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;
        sword.enabled = weapon == WeaponType.Sword;
        shotgun.enabled = weapon == WeaponType.Shotgun;
        Debug.Log($"Equipped: {weapon}");
    }
}