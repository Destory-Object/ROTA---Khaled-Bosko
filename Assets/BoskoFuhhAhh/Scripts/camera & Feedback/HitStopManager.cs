using UnityEngine;
using System.Collections;
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    private float stopEndTime = -1f;
    private bool isStopped = false;

    private void Awake()
    {
        Instance = this;
    }
    public static void RequestHitStop(float duration)
    {
        if (Instance == null) return;

        float requestedEnd = Time.unscaledTime + duration;
        Instance.stopEndTime = Mathf.Max(Instance.stopEndTime, requestedEnd);

        if (!Instance.isStopped)
            Instance.StartCoroutine(Instance.HitStopRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        isStopped = true;
        Time.timeScale = 0f;

        while (Time.unscaledTime < stopEndTime)
            yield return null;

        Time.timeScale = 1f;
        isStopped = false;
        stopEndTime = -1f;
    }
}