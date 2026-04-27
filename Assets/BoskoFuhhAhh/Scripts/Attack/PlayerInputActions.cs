using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour, IContract
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;

    InputAction parryAction;
    private InputAction attackAction;
    private bool isParrying = false;
    [SerializeField] float jumpForce;
    private PlayerController pc;
    [SerializeField] float parryWindowDuration = 0.1f;

    public float attackDistance = 1f;
    private Vector2 attackOffsetFront = new Vector2(1f, 0f);
    private Vector2 attackOffsetUnder = new Vector2(0f, -1f);
    private Rigidbody2D playerRb;

    private void Start()
    {
        parryAction = InputSystem.actions.FindAction("Parry");
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        attackAction = InputSystem.actions.FindAction("Attack");
        if (attackAction != null)
            attackAction.performed += ctx => Attack();
        else
            Debug.LogError("Attack not found");

        parryAction = InputSystem.actions.FindAction("Parry");
    }

    private void OnEnable()
    {
        attackAction?.Enable();
        parryAction?.Enable();
    }

    private void OnDisable()
    {
        attackAction?.Disable();
        parryAction?.Disable();
    }

    public bool IsParrying() => isParrying;

    private void Update()
    {
        if (playerRb != null && attackPoint != null)
        {
            attackPoint.localPosition = playerRb.linearVelocity.y < -0.1f
                ? attackOffsetUnder * attackDistance
                : attackOffsetFront * attackDistance;
        }

        if (parryAction != null && parryAction.WasPerformedThisFrame())
            StartCoroutine(ParryRoutine());
    }

    public void ExecuteAction() { }

    private void Attack()
    {
        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(
            attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            IHealth healthComp = enemy.GetComponent<IHealth>();
            if (healthComp != null)
                //Uses CombatEffects for crit rolls + popup
                CombatEffects.DealDamage(healthComp, damageAmount, enemy.transform.position);
        }
    }

    private IEnumerator ParryRoutine()
    {
        isParrying = true;
        if (pc != null) pc.playerState = "parryState";

        Debug.Log("PARRRRRRY");
        yield return null;

        float timer = 0f;
        while (timer < parryWindowDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
        if (pc != null) pc.playerState = "Normal";
        Debug.Log("No Parry");
        isParrying = false;
        yield return null;
    }

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

    private IEnumerator ParryFreezeFrame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f); // 80ms freeze
        Time.timeScale = 1f;
    }

    public bool IsFacing(Vector2 targetPosition)
{
    float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;
    float dirToTarget = targetPosition.x - transform.position.x;
    return Mathf.Sign(dirToTarget) == Mathf.Sign(facing);
}

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}