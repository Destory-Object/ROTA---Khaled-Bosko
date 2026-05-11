using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionProperty interactAction;
    public GameObject currentInteractable;

    InputAction scanAction;

    private void OnEnable()
    {

        scanAction = InputSystem.actions.FindAction("Interact");
    
       
    }

    private void Update()
    {
        if (scanAction.WasPerformedThisFrame())
        {
            if (currentInteractable != null)
            {
                currentInteractable.GetComponent<IInteractable>()?.Interact();
                currentInteractable = null;
            }
        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            currentInteractable = collision.gameObject;
            Debug.Log("Found interactable: " + collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            
                currentInteractable = null;
            
        }
    }
}