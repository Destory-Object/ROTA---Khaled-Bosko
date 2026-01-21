using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionProperty interactAction;
    private IInteractable currentInteractable;

    InputAction scanAction;

    private void OnEnable()
    {

        scanAction = InputSystem.actions.FindAction("Interact");
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            currentInteractable = collision.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            if (collision.GetComponent<IInteractable>() == currentInteractable)
            {
                currentInteractable = null;
            }
        }
    }
}