using UnityEngine;

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
    [SerializeField] private SlotAudioController audioController;

    private bool isSpinning = false;

    // Spin flow: guard -> validate bet -> deduct bet -> lock input -> generate outcomes -> spin reels.
    // Outcomes are determined upfront (not by the reels) so the RNG is cleanly separated
    // from the animation. Reels merely animate toward their pre-determined targets.
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

    // Callback from ReelManager after all 3 reels stop. Payout is calculated as:
    // multiplier (from PayoutTable) * current bet amount. Winnings are added back to
    // wallet, which fires its own events to update the UI.
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
            audioController?.PlayWinSound();
        }
        else
        {
            audioController?.StopSpinSound();
        }

        leverController.SetInteractable(wallet.CanPlaceBet());
    }
}
