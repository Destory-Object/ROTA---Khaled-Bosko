using UnityEngine;

public class CombatEffects : MonoBehaviour
{
    public static CombatEffects Instance;

    [Header("Damage Popup")]
    [SerializeField] private GameObject damagePopupPrefab;

    [Header("Crit")]
    [SerializeField] private float critChance = 0.2f; //20%
    [SerializeField] private float critMultiplier = 2f; //2x damage

    private void Awake()
    {
        Instance = this;
    }

    public static void DealDamage(IHealth target, int baseDamage, Vector3 popupPosition,
        bool forceCrit = false)
    {
        if (Instance == null || target == null) return;

        bool isCrit = forceCrit || Random.value < Instance.critChance;
        int finalDamage = isCrit
            ? Mathf.RoundToInt(baseDamage * Instance.critMultiplier)
            : baseDamage;

        target.TakeDamage(finalDamage);

        //Notify health bar if one exists on the target
        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono != null)
        {
            EnemyHealthBar healthBar = targetMono.GetComponentInChildren<EnemyHealthBar>();
            if (healthBar != null)
                healthBar.OnDamageTaken();
        }

        Instance.SpawnPopup(finalDamage, isCrit, popupPosition);
    }

    private void SpawnPopup(int damage, bool isCrit, Vector3 position)
    {
        if (damagePopupPrefab == null) return;

        //Randomise position slightly so multiple hits don't stack
        Vector3 offset = new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(0f, 0.3f),
            0f);

        GameObject popup = Instantiate(damagePopupPrefab, position + offset, Quaternion.identity);
        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
            popupScript.Setup(damage, isCrit);
    }
}