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

    [SerializeField] float parryWindowDuration = 0.5f;

    // Offsets for attack point positions
    public float attackDistance = 1f;
    private Vector2 attackOffsetFront = new Vector2(1f, 0f);
    private Vector2 attackOffsetUnder = new Vector2(0f, -1f);

    private Rigidbody2D playerRb;

    Animator ani;

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
        {
            attackAction.performed += ctx => Attack();
        }
        else
        {
            Debug.LogError("Attack not found");
        }
        parryAction = InputSystem.actions.FindAction("Parry");
    }

    private void OnEnable()
    {
        attackAction?.Enable();
        if (parryAction != null)
            parryAction.Enable();
    }

    private void OnDisable()
    {
        attackAction?.Disable();
        if (parryAction != null)
            parryAction.Disable();
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    private void Update()
    {
     
        if (playerRb != null && attackPoint != null)
        {
            if (playerRb.linearVelocity.y < -0.1f)
            {
                
                attackPoint.localPosition = attackOffsetUnder * attackDistance;
               
            }
            else
            {
                
                attackPoint.localPosition = attackOffsetFront * attackDistance;

            }
        }

        if (parryAction != null && parryAction.WasPerformedThisFrame())
        {
            StartCoroutine(ParryRoutine());
        }
    }


    public void ExecuteAction()
    {
        
    }
    private void Attack()
    {
        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            IHealth healthComp = enemy.GetComponent<IHealth>();
            if (healthComp != null)
                healthComp.TakeDamage(damageAmount);

            ani = enemy.gameObject.GetComponent<Animator>();
        }
    }

    private IEnumerator ParryRoutine()
    {
        isParrying = true;
        if (pc != null)
        {
            pc.playerState = "parryState";
        }
        Debug.Log("PARRRRRRY");
        yield return null;

        float timer = 0f;
        while (timer < parryWindowDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;

        if (pc != null)
        {
            pc.playerState = "Normal";
        }
        Debug.Log("No Parry");
        isParrying = false;
        yield return null;
    }

    public void OnEnemyAttackHit()
    {
        if (isParrying)
        {
            Debug.Log("Bra Parry");
        }
        else
        {
            Debug.Log("Player hit");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}