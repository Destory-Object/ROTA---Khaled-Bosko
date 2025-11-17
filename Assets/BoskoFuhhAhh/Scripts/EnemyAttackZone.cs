using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            var playerInput = collision.GetComponent<PlayerInputActions>();
            if(playerInput != null )
            {
                playerInput.OnEnemyAttackHit();
            }
        }
    }
}
