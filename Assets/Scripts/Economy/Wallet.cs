using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }

/// <summary>
/// Manages the player's credit balance and bet amount. Fires events so
/// other systems (UI, audio) can react to balance changes without coupling.
/// </summary>
public class Wallet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int startingCredits = 1000;
    [SerializeField] private int betAmount = 10;

    [Header("Events")]
    public UnityEvent onCreditsChanged;
    public IntUnityEvent onWinAwarded;

    private int credits;

    public int Credits => credits;
    public int BetAmount => betAmount;

    public void SetBetAmount(int amount)
    {
        if (amount <= 0)
            return;
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
