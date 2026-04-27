using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// SETUP:
///   1. Attach to the Player alongside PlayerController
///   2. Set climbableLayers to your Ground/Environment layer
///   3. No child GameObjects needed TACK SÅ MYCKET CLAUDE

public class LedgeGrab : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask climbableLayers;
    [SerializeField] private float wallCheckWidth = 0.15f;
    [SerializeField] private float wallCheckHeight = 0.3f;
    [SerializeField] private float ledgeTopCheckSize = 0.2f;

    [Header("Mantle")]
    [SerializeField] private float mantleUpOffset = 1.2f;
    [SerializeField] private float mantleForwardOffset = 0.3f;
    [SerializeField] private float mantleDuration = 0.18f;

    private PlayerController pc;
    private Rigidbody2D rb;
    private float originalGravityScale;
    private bool isMantling = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (isMantling) return;
        if (pc.playerState == "Climbing") return;
        //if (rb.linearVelocity.y < -1f) return; // falling too fast — skip

        TryGrabLedge();
    }

    private void TryGrabLedge()
    {
        float facing = transform.rotation.eulerAngles.y > 90f ? -1f : 1f;

        Vector2 upperBody = (Vector2)transform.position + new Vector2(0f, 0.4f);
        Vector2 wallCheckCenter = upperBody + new Vector2(facing * (wallCheckWidth * 0.5f), 0f);

        // Wall at upper body level
        bool wallHit = Physics2D.OverlapBox(
            wallCheckCenter,
            new Vector2(wallCheckWidth, wallCheckHeight),
            0f, climbableLayers);

        if (!wallHit) return;

        // Open air above the ledge
        Vector2 aboveLedge = upperBody + new Vector2(facing * wallCheckWidth, wallCheckHeight * 0.5f + 0.1f);
        bool topOpen = !Physics2D.OverlapCircle(aboveLedge, ledgeTopCheckSize, climbableLayers);

        if (!topOpen) return;

        // Ledge found — mantle immediately, no input needed
        RaycastHit2D surface = Physics2D.Raycast(
            aboveLedge + Vector2.up * 0.3f, Vector2.down, 1f, climbableLayers);

        float landY = surface.collider != null
            ? surface.point.y + mantleUpOffset
            : aboveLedge.y + mantleUpOffset;

        Vector2 target = new Vector2(
            transform.position.x + facing * mantleForwardOffset,
            landY);

        StartCoroutine(Mantle(target));
    }

    private IEnumerator Mantle(Vector2 target)
    {
        isMantling = true;
        pc.playerState = "Climbing";

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        Vector2 startPos = transform.position;
        float timer = 0f;

        while (timer < mantleDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, timer / mantleDuration);
            transform.position = Vector2.Lerp(startPos, target, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = Vector2.zero;

        isMantling = false;
        pc.playerState = "Normal";
    }

    private void OnDrawGizmosSelected()
    {
        float facing = Application.isPlaying
            ? (transform.rotation.eulerAngles.y > 90f ? -1f : 1f)
            : 1f;

        Vector2 upperBody = (Vector2)transform.position + new Vector2(0f, 0.4f);
        Vector2 wallCheckCenter = upperBody + new Vector2(facing * (wallCheckWidth * 0.5f), 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheckCenter, new Vector3(wallCheckWidth, wallCheckHeight, 0f));

        Vector2 aboveLedge = upperBody + new Vector2(facing * wallCheckWidth, wallCheckHeight * 0.5f + 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aboveLedge, ledgeTopCheckSize);
    }
}