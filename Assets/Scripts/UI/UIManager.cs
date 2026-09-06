using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Wallet wallet;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI winText;

    // Subscribe to Wallet's credits-changed event so the UI updates automatically
    // whenever a bet is placed or a win is awarded -- no polling needed.
    private void Start()
    {
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
