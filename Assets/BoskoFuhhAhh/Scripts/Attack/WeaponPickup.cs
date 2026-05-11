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

    private void EnablePickup()
    {
        canPickUp = true;
    }

    public void Interact()
    {
        if (!canPickUp) return;

        WeaponManager weaponManager = FindAnyObjectByType<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.PickUp(weaponType);
            Destroy(gameObject);
        }
    }
}