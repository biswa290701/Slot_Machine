using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Popup shown when the player wins. Displays the win amount with Yes/No
/// buttons that both dismiss the popup. Wired by BuildScene.
/// </summary>
public class WinPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI winAmountText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        Hide();
        if (yesButton != null) yesButton.onClick.AddListener(Hide);
        if (noButton != null) noButton.onClick.AddListener(Hide);
    }

    public void ShowPopup(int winAmount)
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (winAmountText != null)
            winAmountText.text = winAmount.ToString();
    }

    public void Hide()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}
