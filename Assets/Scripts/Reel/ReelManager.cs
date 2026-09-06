using UnityEngine;
using System;

/// <summary>
/// Coordinates spinning all three reels. Each reel starts simultaneously and
/// stops after a staggered delay. The onComplete callback fires once after
/// every reel has stopped.
/// </summary>
public class ReelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ReelColumn[] reels;

    private SymbolData[] finalOutcomes;
    private int stoppedCount;
    private Action<SymbolData[]> onComplete;

    public void StartSpin(SymbolData[] outcomes, Action<SymbolData[]> onComplete)
    {
        if (reels == null || reels.Length == 0)
        {
            onComplete?.Invoke(outcomes);
            return;
        }

        if (outcomes == null || outcomes.Length < reels.Length)
        {
            onComplete?.Invoke(outcomes);
            return;
        }

        finalOutcomes = outcomes;
        this.onComplete = onComplete;
        stoppedCount = 0;

        for (int i = 0; i < reels.Length; i++)
        {
            float delay = GetStopDelay(i);
            reels[i].SpinToSymbol(outcomes[i], delay, OnReelStopped);
        }
    }

    private void OnReelStopped()
    {
        stoppedCount++;
        if (stoppedCount >= reels.Length)
        {
            onComplete?.Invoke(finalOutcomes);
        }
    }

    private float GetStopDelay(int reelIndex)
    {
        switch (reelIndex)
        {
            case 0: return GameConstants.REEL_STOP_DELAY_REEL_0;
            case 1: return GameConstants.REEL_STOP_DELAY_REEL_1;
            case 2: return GameConstants.REEL_STOP_DELAY_REEL_2;
            default: return GameConstants.REEL_STOP_DELAY_REEL_2;
        }
    }
}
