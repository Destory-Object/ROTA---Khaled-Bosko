using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damageAmount = 1;
    public PlayerShotgun shotgun; 

    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
            {
                if (shotgun != null)
                    shotgun.RegisterHit(health, damageAmount, collision.transform.position);
                else
                    CombatEffects.DealDamage(health, damageAmount, collision.transform.position);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}