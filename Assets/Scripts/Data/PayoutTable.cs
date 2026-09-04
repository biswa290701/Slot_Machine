using UnityEngine;

[CreateAssetMenu(fileName = "NewPayoutTable", menuName = "Slot Machine/Payout Table")]
public class PayoutTable : ScriptableObject
{
    [Tooltip("All symbols in the game. Index order matters for lookups.")]
    public SymbolData[] symbols;

    [Header("Win Multipliers")]
    [Tooltip("Multiplier when all 3 reels match this symbol. Array index = symbol index.")]
    public int[] threeOfAKindMultipliers;

    // Win condition: all three reels must show the exact same SymbolData reference.
    // We look up the matching symbol's index to find its payout multiplier in the
    // parallel threeOfAKindMultipliers array. Uses reference equality since SymbolData
    // instances are ScriptableObjects shared across reels.
    public int GetPayout(SymbolData symbolA, SymbolData symbolB, SymbolData symbolC)
    {
        if (symbolA == null || symbolB == null || symbolC == null)
            return 0;

        if (symbolA == symbolB && symbolB == symbolC)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i] == symbolA)
                {
                    if (i < threeOfAKindMultipliers.Length)
                        return threeOfAKindMultipliers[i];
                    return 0;
                }
            }
        }

        return 0;
    }
}
