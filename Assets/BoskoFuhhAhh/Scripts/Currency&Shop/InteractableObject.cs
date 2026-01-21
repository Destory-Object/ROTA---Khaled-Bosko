using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public Animator animator;
    public string interactAnimationTrigger = "Interact";
    public int rewardAmount = 10;
    private bool hasInteracted = false;
    private void Start()
    {
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
            animator.SetTrigger(interactAnimationTrigger);
            PlayerCurrency playerCurrency = new PlayerCurrency();
            playerCurrency.AddCurrency(rewardAmount);
  
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
