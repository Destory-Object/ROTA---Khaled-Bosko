using UnityEngine;

public class EnemyHealthCollider : MonoBehaviour, IHealth
{
    [SerializeField] IHealth parentHealth;
    private void Awake()
    {
        parentHealth = transform.parent.GetComponent<IHealth>();
    }
    public int GetHealth()
    {
        return parentHealth.GetHealth();
    }

    public void Kill()
    {
        parentHealth.Kill();
    }

    public void RegenHealth(int amount)
    {
        parentHealth.RegenHealth(amount);
    }

    public void TakeDamage(int amount)
    {
        parentHealth.TakeDamage(amount);
    }
}
