using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum State
{
    None,
    Idle,
    Walking,
    Knockback,
    Dead
}

public class EnemyClass : MonoBehaviour
{
    [Header("Enemy Config")]
    [SerializeField] public State currentState = State.None; // Current state of enemy
    [SerializeField] public float maxHealth = 0; // The max health any enemy can have
    [SerializeField] public float damage = 0; // The damage any enemy can do
    [SerializeField] public float movementSpeed = 0; // How fast each enemy is
     // The rigidbody of each enemy

    [Header("Enemy Layers")]
    // What is declared as ground in the scene
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public LayerMask whatIsPlayer;

    [Header("Runtime")]
    [SerializeField] public float currentHealth = 0; // The current health each enemy has

    [Header("Detection Position")]
    [SerializeField] Transform DetectPointFront;
    [SerializeField] float DPFradius;

    [Header("Detection Position")]
    [SerializeField] Transform DetectPointBack;
    [SerializeField] float DPBradius;

    private void Awake()
    {
        //om det kommer problem flytta detta till void Start
        whatIsGround =  LayerMask.GetMask(new string[] { "ground" });
    }

    private void Start()
    {
        if (maxHealth <= 0) maxHealth = 1;  
    }

    private void Update()
    {
        Detection();
    }

    private void Detection()
    {
        Physics2D.OverlapCircle(DetectPointFront.position, DPFradius, whatIsPlayer);
    }
    private void BackDetection()
    {
        Physics2D.OverlapCircle(DetectPointBack.position, DPBradius, whatIsPlayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(DetectPointFront.position, DPFradius);
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(DetectPointBack.position, DPBradius);
    }



    public virtual void Movement() { throw new System.Exception("Movement needs to be declared on this enemy"); }
    // Movement is declared in each enemy using this script as body (Inherit)
}
