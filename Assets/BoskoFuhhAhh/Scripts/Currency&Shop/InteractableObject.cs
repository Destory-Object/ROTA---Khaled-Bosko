using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public Animator animator;
    public string interactAnimationTrigger = "Interact";
    public int rewardAmount = 10;
    private bool hasInteracted = false;

    PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Interact()
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            Debug.Log("Triggering interact animation");
            animator.SetTrigger(interactAnimationTrigger);
            player.playerCurrency.AddCurrency(rewardAmount);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
