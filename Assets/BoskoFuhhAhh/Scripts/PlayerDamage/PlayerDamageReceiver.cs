using System.Collections;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IHealth
{
    public int playerMaxHealth;
    public int playerCurrentHealth;

    [Header("Invincibility Frames")]
    [SerializeField] private float iFrameDuration = 1f;
    [SerializeField] private float flashSpeed = 10f;
    private bool isInvincible = false;
    private Coroutine iFrameCoroutine;

    [SerializeField] bool canTakeDamage = true;
    private PlayerInputActions inputActions;
    private SpriteRenderer spriteRenderer;
    Animator ani;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private Rigidbody2D rb;
    public bool isKnockedBack = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = GetComponent<PlayerInputActions>();
        playerCurrentHealth = playerMaxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = FindAnyObjectByType<Camera>().gameObject.GetComponent<Animator>();
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (!canTakeDamage || isInvincible) return;

        playerCurrentHealth -= amount;
        //ani.SetTrigger("Camera_SoftShake");
        //ani.SetTrigger("Player_Damage");

        if (iFrameCoroutine != null)
            StopCoroutine(iFrameCoroutine);
        iFrameCoroutine = StartCoroutine(IFrameRoutine());

        StartCoroutine(KnockbackRoutine(sourcePosition));

        if (playerCurrentHealth <= 0)
            Die();
    }

    private IEnumerator IFrameRoutine()
    {
        isInvincible = true;
        float timer = 0f;
        while (timer < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            timer += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(1f / flashSpeed);
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
        iFrameCoroutine = null;
    }
    private IEnumerator KnockbackRoutine(Vector2 sourcePosition)
    {
        isKnockedBack = true;

        float knockDir = transform.position.x > sourcePosition.x ? 1f : -1f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockDir * knockbackForce, knockbackForce * 0.5f),
            ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    public void SwitchDamageRecieve(bool? active)
    {
        if (active != null)
            canTakeDamage = (bool)active;
        else
            canTakeDamage = !canTakeDamage;
    }

    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
            inputActions.OnEnemyAttackHit();
    }

    public void RegenHealth(int amount) =>
        playerCurrentHealth += Mathf.Max(0, amount);

    public int GetHealth() => playerCurrentHealth;

    public void Kill() => Debug.LogWarning("Player should not be killed");
}