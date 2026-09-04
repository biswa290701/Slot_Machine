using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image leverImage;
    [SerializeField] private Button leverButton;

    [Header("Sprites")]
    [SerializeField] private Sprite leverUp;
    [SerializeField] private Sprite leverDown;

    [Header("Timing")]
    [Tooltip("How long the lever stays in the DOWN position before returning to UP")]
    [SerializeField] private float pullDuration = 0.3f;

    private bool isLocked = false;

    private void Awake()
    {
        leverImage.sprite = leverUp;
        leverButton.onClick.AddListener(OnLeverClicked);
    }

    // Lever click handler: lock interaction, show DOWN sprite, trigger spin immediately,
    // then wait before returning to UP. The spin starts at the moment of the pull so
    // reels begin animating while the lever is still visually in the DOWN position.
    private void OnLeverClicked()
    {
        if (isLocked)
            return;

        isLocked = true;
        leverButton.interactable = false;
        gameManager.Spin();
        StartCoroutine(PullSequence());
    }

    // Two-phase coroutine: DOWN for pullDuration, then back to UP.
    // The UP transition is purely visual and does not re-enable interaction --
    // that is controlled externally via SetInteractable() after all reels stop.
    private IEnumerator PullSequence()
    {
        leverImage.sprite = leverDown;
        yield return new WaitForSeconds(pullDuration);
        leverImage.sprite = leverUp;
    }

    // Called by GameManager when all reels stop to re-enable or disable the lever.
    // Uses a separate lock flag so the visual UP/DOWN transition and the click
    // gate are independent -- the lever can return to UP while still locked.
    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            isLocked = false;
            leverButton.interactable = true;
        }
        else
        {
            isLocked = true;
            leverButton.interactable = false;
        }
    }
}
