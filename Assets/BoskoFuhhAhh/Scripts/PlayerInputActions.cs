using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;

    InputAction parryAction;
    private InputAction attackAction;
    private bool isParrying = false;

    [SerializeField] float jumpForce = 7f;


    private PlayerController pc;

    [SerializeField] float parryWindowDuration = 0.5f;

    // Offsets for attack point positions
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

    private void Attack()
    {
        Collider2D[] hitEnemies = AttackUtilities.DetectEnemies(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyPatrol>().TakeDamage(damageAmount);
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

        float timer = 0f;
        while (timer < parryWindowDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        isParrying = false;

        if (pc != null)
        {
            pc.playerState = "Normal";
        }
        Debug.Log("No Parry");
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