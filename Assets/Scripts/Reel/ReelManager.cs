using UnityEngine;
using System;

public class ReelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ReelColumn[] reels;

    private SymbolData[] finalOutcomes;
    private int stoppedCount;
    private Action<SymbolData[]> onComplete;

    // All reels start spinning simultaneously. Each reel stops after a staggered delay,
    // creating the cascading stop effect. The callback fires only after ALL reels have
    // stopped, using a simple counter to synchronize.
    public void StartSpin(SymbolData[] outcomes, Action<SymbolData[]> onComplete)
    {
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
