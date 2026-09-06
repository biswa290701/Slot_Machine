using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Wallet wallet;
    [SerializeField] private ReelManager reelManager;
    [SerializeField] private PayoutTable payoutTable;
    [SerializeField] private SymbolData[] symbols;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WinPopup winPopup;
    [SerializeField] private LeverController leverController;
    [SerializeField] private BetButtonController betButtonController;
    [SerializeField] private SlotAudioController audioController;
    [SerializeField] private WinPresentationController winPresentationController;

    private bool isSpinning = false;
    private Coroutine winSequenceCoroutine;

    public void Spin()
    {
        if (isSpinning)
            return;

        if (!wallet.CanPlaceBet())
            return;

        if (!wallet.PlaceBet())
            return;

        isSpinning = true;
        leverController.SetInteractable(false);
        winPopup.Hide();
        uiManager.SetWinText(0);

        SymbolData[] outcomes = new SymbolData[GameConstants.REEL_COUNT];
        for (int i = 0; i < GameConstants.REEL_COUNT; i++)
        {
            outcomes[i] = SymbolPicker.PickRandom(symbols);
        }

        reelManager.StartSpin(outcomes, OnAllReelsStopped);
        audioController?.StartSpinSound();
    }

    private void OnAllReelsStopped(SymbolData[] results)
    {
        isSpinning = false;

        int payout = payoutTable.GetPayout(results[0], results[1], results[2]);
        int totalWin = payout * wallet.BetAmount;

        if (totalWin > 0)
        {
            wallet.AddCredits(totalWin);
            uiManager.SetWinText(totalWin);
            winPopup.ShowPopup(totalWin);
            winSequenceCoroutine = StartCoroutine(WinSequence(totalWin));
        }
        else
        {
            audioController?.StopSpinSound();
            leverController.ResetLever();
            betButtonController.UnlockButtons();
        }
    }

    private IEnumerator WinSequence(int winAmount)
    {
        winPresentationController.ShowWin(winAmount);
        audioController?.PlayWinSound();

        yield return new WaitUntil(() => !audioController.IsWinSoundPlaying());

        winPresentationController.HideWin();
        leverController.ResetLever();
        betButtonController.UnlockButtons();
        winSequenceCoroutine = null;
    }

    private void OnDestroy()
    {
        if (winSequenceCoroutine != null)
            StopCoroutine(winSequenceCoroutine);
    }
}
