using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour, IContract
{
    [Header("Parry")]
    [SerializeField] private float parryWindowDuration = 0.1f;

    private InputAction parryAction;
    private bool isParrying = false;
    private PlayerController pc;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        parryAction = InputSystem.actions.FindAction("Parry");
    }

    private void OnEnable()
    {
        parryAction?.Enable();
    }

    private void OnDisable()
    {
        parryAction?.Disable();
    }

    private void Update()
    {
        if (parryAction != null && parryAction.WasPerformedThisFrame())
            StartCoroutine(ParryRoutine());
    }

    public bool IsParrying() => isParrying;

    public bool IsFacing(Vector2 targetPosition)
    {
        float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;
        float dirToTarget = targetPosition.x - transform.position.x;
        return Mathf.Sign(dirToTarget) == Mathf.Sign(facing);
    }

    public void ExecuteAction() { }

    public void OnEnemyAttackHit()
    {
        if (isParrying)
            Debug.Log("Bra Parry");
        else
            Debug.Log("Player hit");
    }

    public void OnSuccessfulParry()
    {
        StartCoroutine(ParryFreezeFrame());
    }

    private IEnumerator ParryRoutine()
    {
        isParrying = true;
        if (pc != null) pc.playerState = "parryState";

        float timer = 0f;
        while (timer < parryWindowDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (pc != null) pc.playerState = "Normal";
        isParrying = false;
    }

    private IEnumerator ParryFreezeFrame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;
    }
}