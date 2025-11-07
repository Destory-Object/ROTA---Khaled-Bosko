using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    private float currentHealth;

    private PlayerController pc;
    private GameObject aliveGO, BrokenTopGO, BrokenBotGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBot;

    private Animator AliveAnim;

    private void Start()
    {
       // currentHealth = maxHealth;

     //   pc = GameObject.Find("Player").GetComponent<PlayerController>();

       // aliveGO = transform.Find("Alive").gameObject;
       // BrokenTopGO = transform.Find("BrokenTop").gameObject;
       // BrokenBotGO = transform.Find("BrokenBot").gameObject;

       // rbAlive = aliveGO.GetComponent<Rigidbody2D>();
       // rbBrokenTop = BrokenTopGO.GetComponent<Rigidbody2D>();
       // rbBrokenBot = BrokenBotGO.GetComponent<Rigidbody2D>();

       // aliveGO.SetActive(true);
       // BrokenTopGO.SetActive(false);
       // BrokenBotGO.SetActive(false);

    }

    private void Damage(float damage)
    {
        currentHealth -= damage;
        //playerFacingDirection = pc.GetFacingDirection();
    }



}
