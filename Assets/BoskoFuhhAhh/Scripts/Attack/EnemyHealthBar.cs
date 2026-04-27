using System.Collections;
using UnityEngine;
using UnityEngine.UI;


///   2. Inside it add a background Image and a fill Image (the bar)
///   3. Attach this script to the Canvas, assign the fill Image to healthFill
///   4. Position it above the enemy's head using yOffset
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;              //The colored fill bar Image
    [SerializeField] private float yOffset = 1.5f;         //How far above the enemy

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Visibility")]
    [SerializeField] private float hideDelay = 2f;         
    [SerializeField] private bool alwaysVisible = false;

    private IHealth enemyHealth;
    private int maxHealth;
    private Transform enemyTransform;
    private Coroutine hideCoroutine;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        enemyTransform = transform.parent;
        enemyHealth = enemyTransform.GetComponent<IHealth>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        //Cache max health on start
        maxHealth = enemyHealth != null ? enemyHealth.GetHealth() : 1;

        //Hide bar at start unless always visible
        if (!alwaysVisible)
            canvasGroup.alpha = 0f;

        UpdateBar();
    }

    private void LateUpdate()
    {
        //Keep bar above the enemy, ignore enemy rotation
        transform.position = enemyTransform.position + Vector3.up * yOffset;
        transform.rotation = Quaternion.identity;   // Always face camera
    }

    ///Call this from TakeDamage to refresh the bar
    public void OnDamageTaken()
    {
        UpdateBar();
        ShowBar();
    }

    private void UpdateBar()
    {
        if (enemyHealth == null || healthFill == null) return;

        float fraction = (float)enemyHealth.GetHealth() / maxHealth;
        healthFill.fillAmount = fraction;
        healthFill.color = Color.Lerp(lowHealthColor, fullHealthColor, fraction);
    }

    private void ShowBar()
    {
        canvasGroup.alpha = 1f;

        if (!alwaysVisible)
        {
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        float t = 0f;
        while (t < 0.5f)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}