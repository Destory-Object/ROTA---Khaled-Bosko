using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamageReceiver : MonoBehaviour
{

    [SerializeField] int playerMaxHealth;
    [SerializeField] int playerCurrentHealth;

    private PlayerInputActions inputActions;

    private void Start()
    {
        inputActions = GetComponent<PlayerInputActions>();
        if (inputActions == null)
        {
            Debug.Log("DU glOM script på player");
        }
        
        playerCurrentHealth = playerMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        playerCurrentHealth -= damage;
        Debug.Log("Player took damage, current health: " + playerCurrentHealth);
        if (playerCurrentHealth <= 0)
        {
            Die();
        }
    }




    //private void Update()
    //{
    //    if(playerCurrentHealth  <= 0)
    //    {
    //        Die();
    //    }
    //}

    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("EnemyAttack"))
        {
            inputActions.OnEnemyAttackHit();
        }
    }
}


