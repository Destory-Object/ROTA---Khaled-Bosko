using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamageReceiver : MonoBehaviour, IHealth
{

    [SerializeField] int playerMaxHealth;
    [SerializeField] int playerCurrentHealth;
    [SerializeField] bool canTakeDamage = true;

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
        if (canTakeDamage)
        {
            playerCurrentHealth -= damage;
            Debug.Log("Player took damage, current health: " + playerCurrentHealth);
            if (playerCurrentHealth <= 0)
            {
                Die();
            }
        }
    }




    //private void Update()
    //{
    //    if(playerCurrentHealth  <= 0)
    //    {
    //        Die();
    //    }
    //}

    public void SwitchDamageRecieve(bool? active)
    {
        if (active != null)
            canTakeDamage = (bool)active;
        else
            canTakeDamage = !canTakeDamage;
    }

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

    public void RegenHealth(int amount)
    {
        playerCurrentHealth += Mathf.Max(0, amount);
    }

    public int GetHealth()
    {
        return playerCurrentHealth;
    }

    public void Kill()
    {
        Debug.LogWarning("Player should not be killed");
    }
}


