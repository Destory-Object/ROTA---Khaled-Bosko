using System.Collections;
using TMPro;
using UnityEngine;


public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    [Header("Normal Hit")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float normalFontSize = 4f;

    [Header("Crit Hit")]
    [SerializeField] private Color critColor = Color.yellow;
    [SerializeField] private float critFontSize = 6f;      

    [Header("Animation")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private float lifetime = 0.8f;

    public void Setup(int damage, bool isCrit)
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();

        if (isCrit)
        {
            text.text = $"CRIT! {damage}";
            text.color = critColor;
            text.fontSize = critFontSize;
            transform.localScale = Vector3.one * 1.3f; 
        }
        else
        {
            text.text = damage.ToString();
            text.color = normalColor;
            text.fontSize = normalFontSize;
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float timer = 0f;
        Color startColor = text.color;

        while (timer < lifetime)
        {
            //Float upward
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            //Fade out in the second half of lifetime
            float fadeT = Mathf.Clamp01((timer - lifetime * 0.5f) / (lifetime * 0.5f));
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - fadeT);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}