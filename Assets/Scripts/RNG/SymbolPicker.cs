using UnityEngine;

public static class SymbolPicker
{
    public static SymbolData PickRandom(SymbolData[] symbols)
    {
        if (symbols == null || symbols.Length == 0)
            return null;

        float totalWeight = 0f;
        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] != null)
                totalWeight += symbols[i].dropWeight;
        }

        if (totalWeight <= 0f)
            return symbols[0];

        // Cumulative probability: each symbol occupies a segment of [0, totalWeight]
        // proportional to its dropWeight. We pick a random point and walk forward until
        // we land inside a symbol's segment. This is O(n) but fine for small symbol sets.
        float randomPoint = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] == null)
                continue;

            current += symbols[i].dropWeight;
            if (randomPoint <= current)
                return symbols[i];
        }

        // Floating-point edge case: if randomPoint equals totalWeight exactly, return last symbol
        return symbols[symbols.Length - 1];
    }
}
