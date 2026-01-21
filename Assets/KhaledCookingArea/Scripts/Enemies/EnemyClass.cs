using System.Collections.Generic;
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

    [Header("Runtime")]
    [SerializeField] public float currentHealth = 0; // The current health each enemy has

    [Header("Detection Position")]
    [SerializeField] GameObject DetectPointFront;

    [Header("Detection Position")]
    [SerializeField] GameObject DetectPointBack;

    private void Awake()
    {
        //om det kommer problem flytta detta till void Start
        whatIsGround =  LayerMask.GetMask(new string[] { "ground" });
    }

    private void Start()
    {

        // Get the rigidbody of the enemy if not set
       // if (aliveRb == null) aliveRb = GetComponent<Rigidbody2D>();

        // Set the max health of the enemy if not set
        if (maxHealth <= 0) maxHealth = 1;
        
    }

    public virtual void Movement() { throw new System.Exception("Movement needs to be declared on this enemy"); }
    // Movement is declared in each enemy using this script as body (Inherit)
}
