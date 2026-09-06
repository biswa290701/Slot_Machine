using UnityEngine;
using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Image leverImage;

    [Header("Interaction")]
    [SerializeField] private Button leverButton;

    [Header("Sprites")]
    [SerializeField] private Sprite leverUp;
    [SerializeField] private Sprite leverDown;

    [Header("Down State Offset")]
    [Tooltip("Position offset applied to LeverVisual when the DOWN sprite is active. Adjust in Inspector to align the pulled lever.")]
    [SerializeField] private Vector2 downPositionOffset = Vector2.zero;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SlotAudioController audioController;

    private bool isLocked = false;
    private Vector2 upPosition;

    private void Awake()
    {
        var rect = leverImage.GetComponent<RectTransform>();
        upPosition = rect.anchoredPosition;
        leverImage.sprite = leverUp;
        leverButton.onClick.AddListener(OnLeverClicked);
    }

    private void OnLeverClicked()
    {
        if (isLocked)
            return;

        isLocked = true;
        leverButton.interactable = false;
        ApplyState(leverDown, downPositionOffset);
        audioController?.PlayLeverSound();
        gameManager.Spin();
    }

    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            isLocked = false;
            leverButton.interactable = true;
            ApplyState(leverUp, Vector2.zero);
        }
        else
        {
            isLocked = true;
            leverButton.interactable = false;
        }
    }

    // Immediately sets the sprite, resizes the RectTransform to match the sprite's
    // native dimensions, and applies the given position offset relative to the
    // UP state's anchored position. offset=(0,0) restores the UP position exactly.
    private void ApplyState(Sprite sprite, Vector2 offset)
    {
        leverImage.sprite = sprite;
        var rect = leverImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        rect.anchoredPosition = upPosition + offset;
    }
}
