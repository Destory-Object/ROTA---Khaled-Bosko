using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float pickupDelay = 0.5f;
    private bool canPickUp = false;

    private void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    public void SetPickupDelay(float delay)
    {
        canPickUp = false;
        CancelInvoke(nameof(EnablePickup));
        Invoke(nameof(EnablePickup), delay);
    }

    private void EnablePickup()
    {
        canPickUp = true;
    }

    public void Interact()
    {
        if (!canPickUp) return;

        WeaponManager weaponManager = FindAnyObjectByType<WeaponManager>();
        if (weaponManager == null) return;

        GetComponent<Collider2D>().enabled = false;
        weaponManager.PickUp(weaponType);
        Destroy(gameObject);
    }
}