using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }

public class Wallet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int startingCredits = 1000;
    [SerializeField] private int betAmount = 10;

    [Header("Events")]
    // Events decouple Wallet from UI and other systems. onCreditsChanged fires on any
    // balance mutation (bet or win), so UIManager can bind to it for automatic updates.
    // onWinAwarded passes the win amount for UI feedback (popup, text flash, etc.).
    public UnityEvent onCreditsChanged;
    public IntUnityEvent onWinAwarded;

    private int credits;

    public int Credits => credits;
    public int BetAmount => betAmount;

    public void SetBetAmount(int amount)
    {
        betAmount = amount;
    }

    private void Start()
    {
        credits = startingCredits;
        onCreditsChanged?.Invoke();
    }

    public bool CanPlaceBet()
    {
        return credits >= betAmount;
    }

    public bool PlaceBet()
    {
        if (!CanPlaceBet())
            return false;

        credits -= betAmount;
        onCreditsChanged?.Invoke();
        return true;
    }

    public void AddCredits(int amount)
    {
        if (amount <= 0)
            return;

        credits += amount;
        onCreditsChanged?.Invoke();
        onWinAwarded?.Invoke(amount);
    }
}
