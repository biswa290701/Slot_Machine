using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReelColumn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SymbolData[] allSymbols;
    [SerializeField] private RectTransform reelStripContent;
    [SerializeField] private float symbolHeight = 100f;
    [SerializeField] private int paddingCount = 5;

    private float symbolSpacing;
    private bool isSpinning = false;
    private Coroutine spinCoroutine;
    private Action onStopped;

    private List<Image> symbolImages = new List<Image>();

    private void Awake()
    {
        symbolSpacing = symbolHeight;
    }

    public void SpinToSymbol(SymbolData targetSymbol, float delay, Action onStopped)
    {
        this.onStopped = onStopped;
        isSpinning = true;

        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);

        spinCoroutine = StartCoroutine(SpinSequence(targetSymbol, delay));
    }

    // The strip is laid out with the target symbol at index paddingCount (the middle).
    // We scroll from Y=0 to Y=-(paddingCount+1)*spacing, which moves the strip downward
    // so that the target symbol lands exactly in the visible mask window.
    private IEnumerator SpinSequence(SymbolData targetSymbol, float delay)
    {
        BuildStrip(targetSymbol);
        yield return null;

        float startY = 0f;
        // Scroll past all padding symbols plus one more step to land on the target
        float totalScrollDistance = (paddingCount + 1) * symbolSpacing;
        float targetY = -totalScrollDistance;

        float elapsed = 0f;
        float spinDuration = delay;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            // Hermite interpolation (smoothstep): ease-in then ease-out for natural deceleration
            float smoothT = t * t * (3f - 2f * t);
            float currentY = Mathf.Lerp(startY, targetY, smoothT);
            reelStripContent.anchoredPosition = new Vector2(reelStripContent.anchoredPosition.x, currentY);
            yield return null;
        }

        reelStripContent.anchoredPosition = new Vector2(reelStripContent.anchoredPosition.x, targetY);

        // Bounce effect: overshoot past the target then settle back using a half-sine curve.
        // Sin(0..PI) produces a smooth 0->1->0 pulse, giving a subtle "bounce" feel.
        float bounceDistance = symbolSpacing * GameConstants.REEL_BOUNCE_OVERSHOOT;
        float bounceElapsed = 0f;
        float bounceDuration = GameConstants.REEL_BOUNCE_SETTLE_DURATION;
        float bounceTarget = targetY + bounceDistance;

        while (bounceElapsed < bounceDuration)
        {
            bounceElapsed += Time.deltaTime;
            float t = bounceElapsed / bounceDuration;
            float bounceT = Mathf.Sin(t * Mathf.PI);
            float currentY = Mathf.Lerp(targetY, bounceTarget, bounceT);
            reelStripContent.anchoredPosition = new Vector2(reelStripContent.anchoredPosition.x, currentY);
            yield return null;
        }

        reelStripContent.anchoredPosition = new Vector2(reelStripContent.anchoredPosition.x, targetY);

        isSpinning = false;
        onStopped?.Invoke();
    }

    // Constructs the visual reel strip at runtime. The target symbol is placed at the
    // exact index where it will land in the mask window after scrolling. Symbols above
    // and below are random filler that scroll through during the animation.
    private void BuildStrip(SymbolData targetSymbol)
    {
        foreach (Transform child in reelStripContent)
        {
            Destroy(child.gameObject);
        }
        symbolImages.Clear();

        int totalSymbols = paddingCount * 2 + 1;
        float totalHeight = (totalSymbols - 1) * symbolSpacing;
        reelStripContent.sizeDelta = new Vector2(reelStripContent.sizeDelta.x, totalHeight);
        reelStripContent.anchoredPosition = new Vector2(reelStripContent.anchoredPosition.x, 0f);

        for (int i = 0; i < totalSymbols; i++)
        {
            // Target symbol occupies the exact center position (paddingCount index) so it
            // aligns with the mask window when the scroll reaches its final Y offset.
            SymbolData symbolToShow;
            if (i == paddingCount)
            {
                symbolToShow = targetSymbol;
            }
            else
            {
                symbolToShow = allSymbols[UnityEngine.Random.Range(0, allSymbols.Length)];
            }

            GameObject symbolGO = new GameObject("Symbol_" + i, typeof(RectTransform), typeof(Image));
            symbolGO.transform.SetParent(reelStripContent, false);

            RectTransform rt = symbolGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(symbolHeight, symbolHeight);
            float yPos = -i * symbolSpacing;
            rt.anchoredPosition = new Vector2(0f, yPos);

            Image img = symbolGO.GetComponent<Image>();
            if (symbolToShow != null && symbolToShow.sprite != null)
            {
                img.sprite = symbolToShow.sprite;
                img.preserveAspect = true;
            }

            symbolImages.Add(img);
        }
    }
}
