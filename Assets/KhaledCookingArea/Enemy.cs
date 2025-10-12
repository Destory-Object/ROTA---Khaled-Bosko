using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class Enemy : MonoBehaviour
{
    void Start()
    {
        currentHealth = maxHealth;
        EnemyAI = "Walking";
    }

    [SerializeField] string EnemyAI;
    public Transform enemyPosition;
    public float EnemyViewRange;
    public LayerMask PlayerLayers;

    #region EnemyHealthSystem
    public int maxHealth;
    public int currentHealth;


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

    public GameObject Player;
    public float Speed;

    private float Distance;

    private void Update()
    {
        Distance = Vector2.Distance(transform.position, Player.transform.position);
        Vector2 direction = Player.transform.position - transform.position;
        direction.Normalize();


        switch (EnemyAI)
        {
            case "Walking":
                DoWalk();

                break;

            case "Dodging":
                DoDodge();

                break;

            case "Tackling":
                DoTackling();

                break;
        }
    }

    #region StateSystemFunctions
    void DoWalk()
    {
        Debug.Log("Walking");
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enemyPosition.position, EnemyViewRange, PlayerLayers);

        foreach (Collider2D Player in hitPlayer)
        {
            Debug.Log("We hit" + Player.name);
            //Player.GetComponent<Enemy>().GambleMove();
        }

        GambleMove();

    }

    void DoDodge()
    {
        Debug.Log("Dodging");
        
    }

    void DoTackling()
    {
        Debug.Log("Dodging");
        transform.position = Vector2.MoveTowards(this.transform.position, Player.transform.position, Speed * Time.deltaTime);

    }


    #endregion

    #region AIBehavior
    [SerializeField] int behavior;


    void GambleMove()
    {
        int Chance = Random.Range(1, 4);
        if (Chance == 1)
        {
            behavior = 1;
            EnemyAI = "Tackling";
        }
        if (Chance == 2)
        {
            behavior = 2;
            EnemyAI = "Dodging";
        }
        //if (Chance == 3)
        //{
          //  behavior = 3;
            //EnemyAI = "";
        //}
    }


        #endregion
        private void OnDrawGizmosSelected()
    {
        if (enemyPosition == null)
            return;

        Gizmos.DrawWireSphere(enemyPosition.position, EnemyViewRange);
    }
}