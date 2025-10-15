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

    bool isInGambleMode = false;

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
                StartCoroutine(DoDodge());

                break;

            case "Tackling":
                StartCoroutine(DoTackling());

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
            if (isInGambleMode == false)
            {
                GambleMove();
            }

        }

    }

    IEnumerator DoDodge()
    {
        Debug.Log("Dodging");

        yield return new WaitForSeconds(2);

        EnemyAI = "Walking";
    }

    IEnumerator DoTackling()
    {
        Debug.Log("Tackling");
        transform.position = Vector2.MoveTowards(this.transform.position, Player.transform.position, Speed * Time.deltaTime);

        yield return new WaitForSeconds(3);

        EnemyAI = "Walking";
    }


    #endregion

    #region AIBehavior
    [SerializeField] int behavior;


    void GambleMove()
    {
        isInGambleMode = true;
        int Chance = Random.Range(1, 3);
        if (Chance == 1)
        {
            behavior = 1;
            EnemyAI = "Tackling";

            isInGambleMode = false;
            
            
        }
        if (Chance == 2)
        {
            behavior = 2;
            EnemyAI = "Dodging";

            isInGambleMode = false;
            
            
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