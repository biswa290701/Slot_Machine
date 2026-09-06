using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Central gameplay controller. Manages the spin → reel stop → payout → win/loss
/// cycle and triggers GameOver when credits reach zero.
/// </summary>
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

    private void Start()
    {
        if (wallet == null) Debug.LogError("GameManager: Wallet reference is missing.", this);
        if (reelManager == null) Debug.LogError("GameManager: ReelManager reference is missing.", this);
        if (payoutTable == null) Debug.LogError("GameManager: PayoutTable reference is missing.", this);
        if (uiManager == null) Debug.LogError("GameManager: UIManager reference is missing.", this);
        if (leverController == null) Debug.LogError("GameManager: LeverController reference is missing.", this);
        if (betButtonController == null) Debug.LogError("GameManager: BetButtonController reference is missing.", this);
    }

    public void Spin()
    {
        if (isSpinning)
            return;

        if (wallet == null || !wallet.CanPlaceBet())
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
            CheckGameOver();
        }
    }

    // Win presentation plays while the jackpot audio runs. When the audio
    // finishes, the presentation hides and we check for game over.
    private IEnumerator WinSequence(int winAmount)
    {
        winPresentationController.ShowWin(winAmount);
        audioController?.PlayWinSound();

        yield return new WaitUntil(() => !audioController.IsWinSoundPlaying());

        winPresentationController.HideWin();
        leverController.ResetLever();
        winSequenceCoroutine = null;
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (wallet.Credits <= 0)
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        betButtonController.UnlockButtons();
    }

    private void OnDestroy()
    {
        if (winSequenceCoroutine != null)
            StopCoroutine(winSequenceCoroutine);
    }
}
