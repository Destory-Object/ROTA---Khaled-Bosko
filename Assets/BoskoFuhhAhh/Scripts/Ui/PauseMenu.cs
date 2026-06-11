using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private WeaponManager weaponManager;

    [Header("Slot UI")]
    [SerializeField] private TMP_Text slotOneLabel;
    [SerializeField] private TMP_Text slotTwoLabel;
    [SerializeField] private Button swapButton;

    private InputAction pauseAction;
    private bool isPaused = false;

    private void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        pausePanel.SetActive(false);
        swapButton.onClick.AddListener(OnSwapPressed);
    }

    private void Update()
    {
        if (pauseAction.WasPerformedThisFrame())
            TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
            UpdateSlotLabels();
    }

    private void OnSwapPressed()
    {
        weaponManager.SwapSlots();
        UpdateSlotLabels();
    }

    private void UpdateSlotLabels()
    {
        if (slotOneLabel != null)
            slotOneLabel.text = $"Left Click: {weaponManager.slotOne}";
        if (slotTwoLabel != null)
            slotTwoLabel.text = $"Right Click: {weaponManager.slotTwo}";
    }
}