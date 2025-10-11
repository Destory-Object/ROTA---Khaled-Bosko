using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region EnemyHealthSystem
    public int maxHealth;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        //take damage anim

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");

        //Destroy(gameObject);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        //die anim
        //disable the enemy

    }
    #endregion



}