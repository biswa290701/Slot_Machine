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

    private static readonly Color normalColor = new Color(0.25f, 0.25f, 0.3f, 1f);
    private static readonly Color highlightColor = new Color(0.35f, 0.35f, 0.4f, 1f);
    private static readonly Color pressedColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    private static readonly Color selectedColor = new Color(0.85f, 0.65f, 0.15f, 1f);

    private void Start()
    {
        int count = Mathf.Min(betButtons.Length, betAmounts.Length);
        for (int i = 0; i < count; i++)
        {
            if (betButtons[i] == null) continue;
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

    public void LockButtons()
    {
        isLocked = true;
        foreach (var btn in betButtons)
            if (btn != null) btn.interactable = false;
    }

    public void UnlockButtons()
    {
        isLocked = false;
        foreach (var btn in betButtons)
            if (btn != null) btn.interactable = true;

        UpdateVisuals();
    }

    private void OnBetClicked(int index)
    {
        if (isLocked) return;
        if (index < 0 || index >= betButtons.Length || index >= betAmounts.Length) return;
        if (betButtons[index] == null) return;

        if (wallet == null || wallet.Credits < betAmounts[index])
        {
            betButtons[index].interactable = false;
            betButtons[index].interactable = true;
            return;
        }

        wallet.SetBetAmount(betAmounts[index]);
        uiManager.SetBetText(betAmounts[index]);

        selectedIndex = index;
        UpdateVisuals();

        LockButtons();
        leverController.PlaySpinAnimation();
        gameManager.Spin();
    }

    private void UpdateVisuals()
    {
        int count = Mathf.Min(betButtons.Length, betAmounts.Length);
        for (int i = 0; i < count; i++)
        {
            if (betButtons[i] == null) continue;
            var colors = betButtons[i].colors;
            colors.normalColor = (i == selectedIndex) ? selectedColor : normalColor;
            betButtons[i].colors = colors;
        }
    }
}
