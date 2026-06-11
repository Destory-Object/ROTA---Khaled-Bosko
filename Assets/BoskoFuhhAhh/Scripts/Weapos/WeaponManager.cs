using UnityEngine;

public enum WeaponType { None, Sword, Dagger, HeavySword, Shotgun }

public class WeaponManager : MonoBehaviour
{
    [Header("Melee Components")]
    [SerializeField] private PlayerSword sword;
    [SerializeField] private PlayerDagger dagger;
    [SerializeField] private PlayerHeavySword heavySword;

    [Header("Ranged Components")]
    [SerializeField] private PlayerShotgun shotgun;

    [Header("Charge & Pogo")]
    [SerializeField] private ChargeAttack chargeAttack;
    [SerializeField] private PlayerPogo pogo;

    [Header("Drop Prefabs")]
    [SerializeField] private GameObject swordPickupPrefab;
    [SerializeField] private GameObject daggerPickupPrefab;
    [SerializeField] private GameObject heavySwordPickupPrefab;
    [SerializeField] private GameObject shotgunPickupPrefab;

    [Header("Starting Weapons")]
    [SerializeField] private WeaponType startingSlotOne = WeaponType.None;
    [SerializeField] private WeaponType startingSlotTwo = WeaponType.None;
    
    public WeaponType slotOne = WeaponType.None;
    public WeaponType slotTwo = WeaponType.None;
    public WeaponType currentMelee => IsMelee(slotOne) ? slotOne : IsMelee(slotTwo) ? slotTwo : WeaponType.None;
    public WeaponType currentRanged => IsRanged(slotOne) ? slotOne : IsRanged(slotTwo) ? slotTwo : WeaponType.None;

    private void Start()
    {
        DisableAll();

        if (startingSlotOne != WeaponType.None)
            slotOne = startingSlotOne;
        if (startingSlotTwo != WeaponType.None)
            slotTwo = startingSlotTwo;

        RefreshWeapons();
    }
    public void SwapSlots()
    {
        WeaponType temp = slotOne;
        slotOne = slotTwo;
        slotTwo = temp;
        RefreshWeapons();
        Debug.Log($"Swapped — Slot1: {slotOne} | Slot2: {slotTwo}");
    }

    public void PickUp(WeaponType newWeapon)
    {
        Debug.Log($"PickUp: {newWeapon} | Slot1: {slotOne} | Slot2: {slotTwo}");

        if (IsMelee(newWeapon))
        {
            if (newWeapon == currentMelee) return;
            DropWeapon(currentMelee);


            if (IsMelee(slotOne))
                slotOne = newWeapon;
            else if (IsMelee(slotTwo))
                slotTwo = newWeapon;
            else if (slotOne == WeaponType.None)
                slotOne = newWeapon;
            else
                slotTwo = newWeapon;
        }
        else
        {
            if (newWeapon == currentRanged) return;
            DropWeapon(currentRanged);
            if (IsRanged(slotOne))
                slotOne = newWeapon;
            else if (IsRanged(slotTwo))
                slotTwo = newWeapon;
            else if (slotOne == WeaponType.None)
                slotOne = newWeapon;
            else
                slotTwo = newWeapon;
        }

        RefreshWeapons();
        Debug.Log($"After PickUp — Slot1: {slotOne} | Slot2: {slotTwo}");
    }
    private void RefreshWeapons()
    {
        DisableAll();

        ActivateWeapon(slotOne);
        ActivateWeapon(slotTwo);
    }

    private void ActivateWeapon(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Sword:
                if (sword != null) sword.enabled = true;
                break;
            case WeaponType.Dagger:
                if (dagger != null) dagger.enabled = true;
                if (pogo != null) pogo.enabled = true;
                break;
            case WeaponType.HeavySword:
                if (heavySword != null) heavySword.enabled = true;
                if (chargeAttack != null) chargeAttack.enabled = true;
                break;
            case WeaponType.Shotgun:
                if (shotgun != null) shotgun.enabled = true;
                break;
        }
    }

    private void DisableAll()
    {
        if (sword != null) sword.enabled = false;
        if (dagger != null) dagger.enabled = false;
        if (heavySword != null) heavySword.enabled = false;
        if (shotgun != null) shotgun.enabled = false;
        if (chargeAttack != null) chargeAttack.enabled = false;
        if (pogo != null) pogo.enabled = false;
    }

    private void DropWeapon(WeaponType weapon)
    {
        GameObject prefab = weapon switch
        {
            WeaponType.Sword => swordPickupPrefab,
            WeaponType.Dagger => daggerPickupPrefab,
            WeaponType.HeavySword => heavySwordPickupPrefab,
            WeaponType.Shotgun => shotgunPickupPrefab,
            _ => null
        };

        if (prefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject dropped = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody2D droppedRb = dropped.GetComponent<Rigidbody2D>();
        if (droppedRb != null)
        {
            droppedRb.linearVelocity = Vector2.zero;
            droppedRb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
        }

        WeaponPickup pickup = dropped.GetComponent<WeaponPickup>();
        if (pickup != null)
            pickup.SetPickupDelay(1f);

        if (slotOne == weapon) slotOne = WeaponType.None;
        else if (slotTwo == weapon) slotTwo = WeaponType.None;
    }

    public bool IsMelee(WeaponType w) =>
        w == WeaponType.Sword || w == WeaponType.Dagger || w == WeaponType.HeavySword;

    public bool IsRanged(WeaponType w) => w == WeaponType.Shotgun;
}