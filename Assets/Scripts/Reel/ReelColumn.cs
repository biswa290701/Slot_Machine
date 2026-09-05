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
    [SerializeField] private int paddingCount = 8;

    // Fixed visual sequence for non-target filler symbols during reel spin.
    // Cycles through allSymbols by index. Modify this array to change the scrolling pattern.
    private static readonly int[] visualSequence = { 0, 1, 2, 3 };

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

    // The final scroll offset is computed from actual UI geometry rather than a formula.
    // After BuildStrip() places the target at strip index paddingCount and layout settles,
    // we read the target's world position, convert it to mask-local coords, and compute
    // the exact strip offset needed to place the target's center at the mask's center (y=0).
    // This works regardless of mask size, anchor, pivot, or manual Inspector adjustments.
    private IEnumerator SpinSequence(SymbolData targetSymbol, float delay)
    {
        RectTransform targetRT = BuildStrip(targetSymbol);
        yield return null;

        RectTransform maskRect = reelStripContent.parent as RectTransform;
        Vector3 targetWorldPos = targetRT.TransformPoint(Vector3.zero);
        Vector3 targetMaskLocal = maskRect.InverseTransformPoint(targetWorldPos);

        float startY = 0f;
        float targetY = -targetMaskLocal.y;

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

    // Constructs the visual reel strip at runtime. Returns the target symbol's RectTransform
    // so SpinSequence can compute the exact scroll offset from actual geometry.
    private RectTransform BuildStrip(SymbolData targetSymbol)
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

        RectTransform targetRT = null;

        for (int i = 0; i < totalSymbols; i++)
        {
            SymbolData symbolToShow;
            if (i == paddingCount)
            {
                symbolToShow = targetSymbol;
            }
            else
            {
                symbolToShow = allSymbols[visualSequence[i % visualSequence.Length]];

                // The target is randomly selected independently of the deterministic
                // visual sequence, which can create duplicate adjacent symbols at the
                // target boundary. When a filler equals the target and is adjacent to it,
                // deterministically advance through the visual sequence to find a symbol
                // that differs from both the target and the other neighbor.
                if (symbolToShow == targetSymbol)
                {
                    int targetPos = paddingCount;
                    if (i == targetPos - 1 || i == targetPos + 1)
                    {
                        int otherNeighbor = (i < targetPos) ? i - 1 : i + 1;
                        SymbolData otherSymbol = allSymbols[visualSequence[otherNeighbor % visualSequence.Length]];

                        for (int offset = 1; offset < visualSequence.Length; offset++)
                        {
                            SymbolData candidate = allSymbols[visualSequence[(i + offset) % visualSequence.Length]];
                            if (candidate != targetSymbol && candidate != otherSymbol)
                            {
                                symbolToShow = candidate;
                                break;
                            }
                        }
                    }
                }
            }

            GameObject symbolGO = new GameObject("Symbol_" + i, typeof(RectTransform), typeof(Image));
            symbolGO.transform.SetParent(reelStripContent, false);

            RectTransform rt = symbolGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(symbolHeight, symbolHeight);
            float yPos = -i * symbolSpacing;
            rt.anchoredPosition = new Vector2(0f, yPos);

            if (i == paddingCount)
                targetRT = rt;

            Image img = symbolGO.GetComponent<Image>();
            if (symbolToShow != null && symbolToShow.sprite != null)
            {
                img.sprite = symbolToShow.sprite;
                img.preserveAspect = true;
            }

            symbolImages.Add(img);
        }

        return targetRT;
    }
}
