using UnityEngine;
using TMPro;

/// <summary>
/// Updates the bottom UI bar (credits, bet, win) from Wallet state.
/// Subscribes to Wallet.onCreditsChanged for automatic updates.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Wallet wallet;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI winText;

    private void Start()
    {
        if (wallet == null)
        {
            Debug.LogError("UIManager: Wallet reference is missing.", this);
            return;
        }

        wallet.onCreditsChanged.AddListener(UpdateCreditsDisplay);
        UpdateCreditsDisplay();
        UpdateBetDisplay();
        SetWinText(0);
    }

    public void UpdateCreditsDisplay()
    {
        if (creditsText != null)
            creditsText.text = wallet.Credits.ToString();
    }

    private void UpdateBetDisplay()
    {
        if (betText != null)
            betText.text = wallet.BetAmount.ToString();
    }

    public void SetWinText(int amount)
    {
        if (winText != null)
            winText.text = amount > 0 ? amount.ToString() : "";
    }

    public void SetBetText(int amount)
    {
        if (betText != null)
            betText.text = amount.ToString();
    }
}
