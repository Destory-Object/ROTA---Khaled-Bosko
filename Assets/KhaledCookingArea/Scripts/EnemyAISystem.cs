using UnityEngine;
using System.Collections;

public class EnemyAISystem : MonoBehaviour
{
    public class EnemyAIController : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;

        [SerializeField] private string EnemyAI;
        public Transform enemyPosition;
        public float EnemyViewRange;
        public LayerMask PlayerLayers;
        public GameObject Player;
        public float Speed;
        private float Distance;

        private bool isInGambleMode = false;
        private bool isPerformingAction = false;

        [SerializeField] private int behavior;

        void Start()
        {
            currentHealth = maxHealth;
            EnemyAI = "Walking";
        }

        void Update()
        {
            if (isPerformingAction) return;

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

                case "Hesitating":
                    StartCoroutine(DoHesitate());
                    break;
            }
        }

        #region Enemy Health System

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            // TODO: Trigger damage animation

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Enemy died");
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;

            // TODO: Trigger death animation
        }

        #endregion

        #region AI Behavior

        void DoWalk()
        {
            Debug.Log("Walking");

            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(enemyPosition.position, EnemyViewRange, PlayerLayers);

            foreach (Collider2D player in hitPlayer)
            {
                Debug.Log("We hit " + player.name);

                if (!isInGambleMode)
                {
                    GambleMove();
                }
            }
        }

        IEnumerator DoDodge()
        {
            isPerformingAction = true;

            Debug.Log("Dodging");

            // TODO: Add dodge animation/movement

            yield return new WaitForSeconds(2f);

            EnemyAI = "Walking";
            isPerformingAction = false;
        }

        IEnumerator DoTackling()
        {
            isPerformingAction = true;

            Debug.Log("Tackling");

            float elapsed = 0f;
            float duration = 3f;

            while (elapsed < duration)
            {
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, Speed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            EnemyAI = "Walking";
            isPerformingAction = false;
        }

        IEnumerator DoHesitate()
        {
            isPerformingAction = true;

            Debug.Log("Hesitating...");

            // Idle behavior, do nothing for a moment
            yield return new WaitForSeconds(1.5f);

            EnemyAI = "Walking";
            isPerformingAction = false;
        }

        void GambleMove()
        {
            isInGambleMode = true;

            int chance = Random.Range(1, 4); // 1 to 3

            switch (chance)
            {
                case 1:
                    behavior = 1;
                    EnemyAI = "Tackling";
                    break;
                case 2:
                    behavior = 2;
                    EnemyAI = "Dodging";
                    break;
                case 3:
                    behavior = 3;
                    EnemyAI = "Hesitating"; // New "Idle"-like state
                    break;
            }

            isInGambleMode = false;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            if (enemyPosition == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemyPosition.position, EnemyViewRange);
        }
    }
}
