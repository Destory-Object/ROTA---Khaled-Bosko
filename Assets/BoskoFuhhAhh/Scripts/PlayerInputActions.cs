using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;


public class PlayerInputActions : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int damageAmount;
   
    InputAction parryAction;
    private InputAction attackAction;

    private PlayerController pc;

    private bool isParrying = false;
    [SerializeField] float parryWindowDuration = 0.5f;
        void Start()
    {
        parryAction = InputSystem.actions.FindAction("Parry");
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
            Debug.LogError("Attack not found u DWEEB");
        }
        parryAction = InputSystem.actions.FindAction("Parry");
    }

    private void OnEnable()
    {
        attackAction?.Enable(); // ? betyder att den kan ha värdet null
        if (parryAction != null)
            parryAction.Enable();
    }

    private void OnDisable()
    {
        attackAction?.Disable();
        if (parryAction != null)
            parryAction.Disable();
           
    }

    private void Update()
    {
        if (parryAction != null && parryAction.WasPerformedThisFrame())
        {
            StartCoroutine(ParryRoutine());
        }
    }


    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
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
        //67
        Debug.Log("PARRRRRRY");

        float timer = 0f; 
        while (timer < parryWindowDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
           isParrying = false;

        if(pc != null)
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
        }else
        {
            Debug.Log("´Player hit");
        }
    }




    //IEnumerator Parry()
    //{

    //    FindAnyObjectByType<PlayerController>().playerState = "Parry";
        
        
    //    yield return new WaitForSeconds(0.3f);


    //    FindAnyObjectByType<PlayerController>().playerState = "Normal";

    //}

    void ReadPlayerInputs()
    {

        
        
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}