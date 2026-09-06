using UnityEngine;

/// <summary>
/// Weighted random symbol selection. Each symbol's dropWeight defines its
/// relative probability: higher weight = more common drop. Null symbols and
/// non-positive weights are ignored during selection.
/// </summary>
public static class SymbolPicker
{
    public static SymbolData PickRandom(SymbolData[] symbols)
    {
        if (symbols == null || symbols.Length == 0)
            return null;

        // Sum only valid (non-null, positive-weight) symbols.
        float totalWeight = 0f;
        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] != null && symbols[i].dropWeight > 0f)
                totalWeight += symbols[i].dropWeight;
        }

        // No valid symbols to pick from.
        if (totalWeight <= 0f)
            return null;

        // Cumulative probability: each symbol occupies a segment of [0, totalWeight]
        // proportional to its dropWeight. Pick a random point and walk forward until
        // we land inside a symbol's segment. O(n) but fine for small symbol sets.
        float randomPoint = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] == null || symbols[i].dropWeight <= 0f)
                continue;

            current += symbols[i].dropWeight;
            if (randomPoint <= current)
                return symbols[i];
        }

        // Floating-point edge case: randomPoint equals totalWeight exactly.
        return symbols[symbols.Length - 1];
    }
}
