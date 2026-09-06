using UnityEngine;
using TMPro;

/// <summary>
/// Controls the win presentation UI shown above the slot machine.
/// Displays "YOU WIN {amount} CREDITS" during jackpot audio playback.
/// GameManager drives the show/hide timing — this class is presentation only.
/// </summary>
public class WinPresentationController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject winPresentationRoot;
    [SerializeField] private TextMeshProUGUI winText;

    public bool IsShowing { get; private set; }

    public void ShowWin(int amount)
    {
        if (winText != null)
            winText.text = $"YOU WIN {amount} CREDITS";

        if (winPresentationRoot != null)
            winPresentationRoot.SetActive(true);

        IsShowing = true;
    }

    public void HideWin()
    {
        if (winPresentationRoot != null)
            winPresentationRoot.SetActive(false);

        IsShowing = false;
    }
}
