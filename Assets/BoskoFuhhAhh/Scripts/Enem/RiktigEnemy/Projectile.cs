using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInteraction playerInteraction = collision.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                if (playerInteraction.GetComponent<PlayerInputActions>().IsParrying())
                {
                    Debug.Log("Projectile parried!");
                    Destroy(gameObject);
                    //trigger parry feedback here
                    return;
                }
            }
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}