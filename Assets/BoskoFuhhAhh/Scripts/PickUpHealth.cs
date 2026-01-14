using UnityEngine;

public class PickUpHealth : MonoBehaviour
{
    //public int healthAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerDamageReceiver playerMaxHealth = collision.GetComponent<PlayerDamageReceiver>();
            if (playerMaxHealth != null)
            {

                playerMaxHealth.playerCurrentHealth = playerMaxHealth.playerMaxHealth;
                Destroy(gameObject);
                Debug.Log("BigPotHeal");
            }
        }
    }

}
