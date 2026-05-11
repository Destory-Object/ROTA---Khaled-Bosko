using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;              
    [SerializeField] private float yOffset = 1.5f;         

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


        maxHealth = enemyHealth != null ? enemyHealth.GetHealth() : 1;


        if (!alwaysVisible)
            canvasGroup.alpha = 0f;

        UpdateBar();
    }

    private void LateUpdate()
    {

        transform.position = enemyTransform.position + Vector3.up * yOffset;
        transform.rotation = Quaternion.identity;   // Always face camera
    }


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