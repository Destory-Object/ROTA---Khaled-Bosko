using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDamageReceiver : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private void Start()
    {
        inputActions = GetComponent<PlayerInputActions>();
        if (inputActions == null)
        {
            Debug.Log("DU glOM script på player");
        }
    }
        private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("EnemyAttack"))
        {
            inputActions.OnEnemyAttackHit();
        }
    }
}


