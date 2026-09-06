using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the three bet buttons (10, 20, 50). Each click sets the bet amount
/// in Wallet, triggers the lever spin animation, and starts the spin via GameManager.
/// Buttons are locked during spins and re-enabled when reels finish.
/// </summary>
public class BetButtonController : MonoBehaviour
{
    [Header("Bet Buttons")]
    [SerializeField] private Button[] betButtons;
    [SerializeField] private int[] betAmounts = { 10, 20, 50 };

    [Header("References")]
    [SerializeField] private Wallet wallet;
    [SerializeField] private LeverController leverController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;

    private bool isLocked = false;
    private int selectedIndex = 0;

    // Button color scheme: normal, hover, pressed, selected (gold highlight)
    private static readonly Color normalColor = new Color(0.25f, 0.25f, 0.3f, 1f);
    private static readonly Color highlightColor = new Color(0.35f, 0.35f, 0.4f, 1f);
    private static readonly Color pressedColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    private static readonly Color selectedColor = new Color(0.85f, 0.65f, 0.15f, 1f);

    private void Start()
    {
        for (int i = 0; i < betButtons.Length; i++)
        {
            int index = i;
            betButtons[i].onClick.AddListener(() => OnBetClicked(index));

            ColorBlock cb = betButtons[i].colors;
            cb.normalColor = normalColor;
            cb.highlightedColor = highlightColor;
            cb.pressedColor = pressedColor;
            cb.selectedColor = selectedColor;
            cb.fadeDuration = 0.1f;
            betButtons[i].colors = cb;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Lock all bet buttons (called when a spin starts).
    /// </summary>
    public void LockButtons()
    {
        isLocked = true;
        foreach (var btn in betButtons)
            btn.interactable = false;
    }

    /// <summary>
    /// Unlock all bet buttons (called when reels finish).
    /// Maintains the current selection highlight.
    /// </summary>
    public void UnlockButtons()
    {
        isLocked = false;
        foreach (var btn in betButtons)
            btn.interactable = true;

        UpdateVisuals();
    }

    private void OnBetClicked(int index)
    {
        if (isLocked)
            return;

        // Validate credits before doing anything
        if (wallet.Credits < betAmounts[index])
        {
            // Brief visual feedback: disable then re-enable to flash the button
            betButtons[index].interactable = false;
            betButtons[index].interactable = true;
            return;
        }

        // Set bet amount in Wallet
        wallet.SetBetAmount(betAmounts[index]);

        // Update bet display
        uiManager.SetBetText(betAmounts[index]);

        // Track selection
        selectedIndex = index;
        UpdateVisuals();

        // Lock buttons immediately to prevent double-clicks
        LockButtons();

        // Trigger lever DOWN animation
        leverController.PlaySpinAnimation();

        // Trigger the existing spin flow (Wallet deducts bet, reels spin, payout runs)
        gameManager.Spin();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < betButtons.Length; i++)
        {
            var colors = betButtons[i].colors;
            colors.normalColor = (i == selectedIndex) ? selectedColor : normalColor;
            betButtons[i].colors = colors;
        }
    }
}
