using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual-only lever controller. Shows UP sprite normally, DOWN sprite when spin starts.
/// BetButtonController triggers PlaySpinAnimation(); GameManager triggers ResetLever().
/// No user interaction — the lever is purely decorative feedback.
/// </summary>
public class LeverController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Image leverImage;
    [SerializeField] private Sprite leverUpSprite;
    [SerializeField] private Sprite leverDownSprite;

    [Header("Position")]
    [SerializeField] private Vector2 downPositionOffset;

    [Header("Audio")]
    [SerializeField] private SlotAudioController audioController;

    private RectTransform leverRect;
    private Vector2 basePosition;
    private bool isDown = false;

    private void Awake()
    {
        leverRect = leverImage.GetComponent<RectTransform>();
        basePosition = leverRect.anchoredPosition;
        leverImage.sprite = leverUpSprite;
        isDown = false;
    }

    /// <summary>
    /// Animates lever to DOWN state. Called by BetButtonController on bet click.
    /// </summary>
    public void PlaySpinAnimation()
    {
        if (isDown) return;
        isDown = true;
        leverImage.sprite = leverDownSprite;
        leverRect.anchoredPosition = basePosition + downPositionOffset;
    }

    /// <summary>
    /// Resets lever to UP state. Called by GameManager when all reels stop.
    /// </summary>
    public void ResetLever()
    {
        if (!isDown) return;
        isDown = false;
        leverImage.sprite = leverUpSprite;
        leverRect.anchoredPosition = basePosition;
    }

    /// <summary>
    /// Kept for backward compatibility with GameManager calls.
    /// Does nothing — lever state is now managed by PlaySpinAnimation/ResetLever.
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (interactable)
            ResetLever();
        else
            PlaySpinAnimation();
    }
}
