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
    [SerializeField] public float maxHealth     = 0; // The max health any enemy can have
    [SerializeField] public float damage        = 0; // The damage any enemy can do
    [SerializeField] public float movementSpeed = 0; // How fast each enemy is
    [SerializeField] public Rigidbody2D aliveRb; // The rigidbody of each enemy

    [Header("Enemy Layers")]
    // What is declared as ground in the scene
    [SerializeField] public LayerMask whatIsGround = LayerMask.GetMask(new string[] { "Default" });

    [Header("Runtime")]
    [SerializeField] public float currentHealth = 0; // The current health each enemy has

    private void Start()
    {
        // Toggle the state to idle if not already set
        if (currentState == State.None) currentState = State.Idle;

        // Get the rigidbody of the enemy if not set
        if (aliveRb == null) aliveRb = GetComponent<Rigidbody2D>();

        // Set the max health of the enemy if not set
        if (maxHealth <= 0) maxHealth = 1;
    }

    public virtual void Movement() { throw new System.Exception("Movement needs to be declared on this enemy"); }
    // Movement is declared in each enemy using this script as body (Inherit)
}
