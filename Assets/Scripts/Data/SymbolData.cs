using UnityEngine;

[CreateAssetMenu(fileName = "NewSymbol", menuName = "Slot Machine/Symbol Data")]
public class SymbolData : ScriptableObject
{
    [Header("Visuals")]
    public Sprite sprite;
    public string symbolName;

    [Header("Gameplay")]
    [Tooltip("Drop weight for this symbol (higher = more common)")]
    public float dropWeight = 1f;

    [Tooltip("Payout multiplier when 3 of this symbol land on the payline")]
    public int payoutMultiplier = 1;
}
