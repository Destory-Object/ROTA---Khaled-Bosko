using System.Collections;
using UnityEngine;

public class SwordSlashEffect : MonoBehaviour
{
    [Header("Arc Shape")]
    [SerializeField] private float arcRadius = 1.5f;
    [SerializeField] private float arcAngle = 120f;
    [SerializeField] private int arcSegments = 20;
    [SerializeField] private Vector2 arcOffset = new Vector2(0.3f, 0f);

    [Header("Timing")]
    [SerializeField] private float sweepDuration = 0.12f;
    [SerializeField] private float fadeDuration = 0.1f;

    [Header("Visuals")]
    [SerializeField] private float coreWidth = 0.35f;
    [SerializeField] private float glowWidth = 0.7f;
    [SerializeField] private float outerWidth = 1.1f;
    [SerializeField] private Color coreColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color glowColor = new Color(0.4f, 0.8f, 1f, 0.7f);
    [SerializeField] private Color outerColor = new Color(0.2f, 0.5f, 1f, 0.25f);
    [SerializeField] private Material lineMaterial;

    private LineRenderer coreLine;
    private LineRenderer glowLine;
    private LineRenderer outerLine;
    private PlayerController pc;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        coreLine  = CreateLine("SlashCore",  coreWidth,  11);
        glowLine  = CreateLine("SlashGlow",  glowWidth,  10);
        outerLine = CreateLine("SlashOuter", outerWidth,  9);
    }

    private LineRenderer CreateLine(string objName, float width, int sortOrder)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.positionCount = arcSegments + 1;
        lr.startWidth = width;
        lr.endWidth = width * 0.4f;
        lr.useWorldSpace = true;
        lr.sortingLayerName = "Default"; // match your sprites layer
        lr.sortingOrder = sortOrder;
        lr.material = lineMaterial != null
            ? lineMaterial
            : new Material(Shader.Find("Sprites/Default"));
        lr.enabled = false;
        return lr;
    }

    public void PlaySlash()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(SlashRoutine());
    }

    private IEnumerator SlashRoutine()
    {
        coreLine.enabled  = true;
        glowLine.enabled  = true;
        outerLine.enabled = true;

        float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;

        // Sweep
        float sweepTimer = 0f;
        while (sweepTimer < sweepDuration)
        {
            float t = sweepTimer / sweepDuration;
            DrawArc(coreLine,  facing, t, coreWidth,  coreWidth  * 0.4f);
            DrawArc(glowLine,  facing, t, glowWidth,  glowWidth  * 0.4f);
            DrawArc(outerLine, facing, t, outerWidth, outerWidth * 0.4f);
            SetColors(t);
            sweepTimer += Time.deltaTime;
            yield return null;
        }

        DrawArc(coreLine,  facing, 1f, coreWidth,  coreWidth  * 0.4f);
        DrawArc(glowLine,  facing, 1f, glowWidth,  glowWidth  * 0.4f);
        DrawArc(outerLine, facing, 1f, outerWidth, outerWidth * 0.4f);

        // Fade
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            float t = fadeTimer / fadeDuration;
            coreLine.startColor  = Color.Lerp(coreColor,  Color.clear, t);
            coreLine.endColor    = Color.Lerp(coreColor,  Color.clear, t);
            glowLine.startColor  = Color.Lerp(glowColor,  Color.clear, t);
            glowLine.endColor    = Color.Lerp(glowColor,  Color.clear, t);
            outerLine.startColor = Color.Lerp(outerColor, Color.clear, t);
            outerLine.endColor   = Color.Lerp(outerColor, Color.clear, t);
            fadeTimer += Time.deltaTime;
            yield return null;
        }

        coreLine.enabled  = false;
        glowLine.enabled  = false;
        outerLine.enabled = false;
    }

    private void SetColors(float sweepT)
    {
        // Briefly flash brighter at the start of the swing
        float brightness = Mathf.Lerp(1.3f, 1f, sweepT);

        coreLine.startColor  = coreColor  * brightness;
        coreLine.endColor    = new Color(coreColor.r,  coreColor.g,  coreColor.b,  0f);
        glowLine.startColor  = glowColor  * brightness;
        glowLine.endColor    = new Color(glowColor.r,  glowColor.g,  glowColor.b,  0f);
        outerLine.startColor = outerColor * brightness;
        outerLine.endColor   = new Color(outerColor.r, outerColor.g, outerColor.b, 0f);
    }

    private void DrawArc(LineRenderer lr, float facing, float sweepT, float startWidth, float endWidth)
    {
        float halfArc = arcAngle / 2f;
        float currentArc = Mathf.Lerp(-halfArc, halfArc, sweepT);
        int pointsToDraw = Mathf.Max(2, Mathf.RoundToInt(sweepT * arcSegments));

        lr.positionCount = pointsToDraw;
        lr.startWidth = startWidth;
        lr.endWidth = endWidth;

        Vector2 offset = new Vector2(arcOffset.x * facing, arcOffset.y);
        Vector3 center = transform.position + (Vector3)offset;

        for (int i = 0; i < pointsToDraw; i++)
        {
            float segT = (float)i / (pointsToDraw - 1);
            float angle = Mathf.Lerp(-halfArc, currentArc, segT);
            float adjustedAngle = facing > 0 ? angle : 180f - angle;
            float rad = adjustedAngle * Mathf.Deg2Rad;

            Vector3 point = center + new Vector3(
                Mathf.Cos(rad) * arcRadius,
                Mathf.Sin(rad) * arcRadius,
                0f);

            lr.SetPosition(i, point);
        }
    }
}