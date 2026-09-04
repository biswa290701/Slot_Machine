using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        yesButton.onClick.AddListener(Hide);
        noButton.onClick.AddListener(Hide);
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
