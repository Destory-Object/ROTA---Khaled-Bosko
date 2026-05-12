using UnityEngine;

public class ScanEnemy : MonoBehaviour, IInteractable
{
    [Header("Scan")]
    [SerializeField] private int minCurrency = 10;
    [SerializeField] private int maxCurrency = 120;
    [SerializeField] private float successChance = 0.7f;

    private bool hasBeenScanned = false;
    private bool isDead = false;
    private PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    public void OnEnemyDied()
    {
        isDead = true;
        gameObject.tag = "Interactable";
    }

    public void Interact()
    {
        if (!isDead)
        {
            Debug.Log("Target still alive — can't scan.");
            return;
        }

        if (hasBeenScanned)
        {
            Debug.Log("Already scanned this target.");
            return;
        }

        hasBeenScanned = true;

        if (Random.value <= successChance)
        {
            int reward = Random.Range(minCurrency, maxCurrency + 1);
            player.playerCurrency.AddCurrency(reward);
            Debug.Log($"Scan successful! Recovered {reward} credits.");
        }
        else
        {
            Debug.Log("Scan failed — data corrupted.");
        }
    }
}