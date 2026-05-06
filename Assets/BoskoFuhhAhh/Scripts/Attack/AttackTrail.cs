using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = GetComponentInParent<PlayerInputActions>();
        trail.emitting = false;
    }

    private void Update()
    {

    }
}