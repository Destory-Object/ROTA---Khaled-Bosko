using System.Collections;
using TMPro;
using UnityEngine;

public class ScanPopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [Header("Success")]
    [SerializeField] private Color successColor = new Color(0f, 1f, 0.8f); // cyan
    [Header("Fail")]
    [SerializeField] private Color failColor = new Color(1f, 0.2f, 0.2f); // red
    [Header("Animation")]
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 2f;

    public void Setup(bool success, int amount)
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();

        if (success)
        {
            text.text = $"> DATA RECOVERED\n+ {amount} CR";
            text.color = successColor;
        }
        else
        {
            text.text = "> DATA CORRUPTED\n+ 0 CR";
            text.color = failColor;
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float timer = 0f;
        Color startColor = text.color;

        while (timer < lifetime)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            float fadeT = Mathf.Clamp01((timer - lifetime * 0.5f) / (lifetime * 0.5f));
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - fadeT);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}