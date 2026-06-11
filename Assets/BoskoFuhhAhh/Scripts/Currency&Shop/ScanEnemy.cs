using UnityEngine;

public class ScanEnemy : MonoBehaviour, IInteractable
{
    [Header("Scan")]
    [SerializeField] private int minCurrency = 10;
    [SerializeField] private int maxCurrency = 120;
    [SerializeField] private float successChance = 0.7f;

    [Header("Feedback")]
    [SerializeField] private GameObject scanPopupPrefab;

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
            SpawnPopup(true, reward);
        }
        else
        {
            SpawnPopup(false, 0);
        }
    }

    private void SpawnPopup(bool success, int amount)
    {
        if (scanPopupPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        GameObject popup = Instantiate(scanPopupPrefab, spawnPos, Quaternion.identity);
        popup.GetComponent<ScanPopup>()?.Setup(success, amount);
    }
}