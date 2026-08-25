// CHANGES: shake now applies in LateUpdate (after cameraMovement's Update
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Presets")]
    [SerializeField] private float softMagnitude = 0.1f;
    [SerializeField] private float softDuration = 0.15f;
    [SerializeField] private float hardMagnitude = 0.3f;
    [SerializeField] private float hardDuration = 0.25f;

    private float shakeTimer = 0f;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;
    private float seedX;
    private float seedY;

    private void Awake()
    {
        Instance = this;
    }

    public static void SoftShake() => Instance?.Shake(Instance.softMagnitude, Instance.softDuration);
    public static void HardShake() => Instance?.Shake(Instance.hardMagnitude, Instance.hardDuration);

    public void Shake(float magnitude, float duration)
    {
        // Only overwrite if the new shake is stronger/longer, so a small hit
        // doesn't cut off a bigger one already in progress.
        if (magnitude * duration >= shakeMagnitude * (shakeDuration - shakeTimer))
        {
            shakeMagnitude = magnitude;
            shakeDuration = duration;
            shakeTimer = 0f;
            seedX = Random.Range(0f, 100f);
            seedY = Random.Range(0f, 100f);
        }
    }

    
    private void LateUpdate()
    {
        if (shakeTimer >= shakeDuration) return;

        float falloff = 1f - (shakeTimer / shakeDuration);
        float offsetX = (Mathf.PerlinNoise(seedX, Time.unscaledTime * 25f) - 0.5f) * 2f * shakeMagnitude * falloff;
        float offsetY = (Mathf.PerlinNoise(seedY, Time.unscaledTime * 25f) - 0.5f) * 2f * shakeMagnitude * falloff;

        transform.position += new Vector3(offsetX, offsetY, 0f);

        shakeTimer += Time.unscaledDeltaTime;
    }
}