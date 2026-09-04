using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateSlotAssets
{
    private const string SYMBOLS_DIR = "Assets/Data/Symbols";
    private const string PAYOUT_DIR = "Assets/Data/Payout";

    [MenuItem("Tools/Create Slot Machine Assets")]
    public static void CreateAll()
    {
        EnsureDirectories();

        SymbolData s1 = CreateSymbol("Symbol_1", "Assets/Art/Symbols/slot-symbol1.png", 1f, 2);
        SymbolData s2 = CreateSymbol("Symbol_2", "Assets/Art/Symbols/slot-symbol2.png", 1f, 5);
        SymbolData s3 = CreateSymbol("Symbol_3", "Assets/Art/Symbols/slot-symbol3.png", 1f, 10);
        SymbolData s4 = CreateSymbol("Symbol_4", "Assets/Art/Symbols/slot-symbol4.png", 1f, 25);

        CreatePayoutTable(s1, s2, s3, s4);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slot machine assets created successfully.");
    }

    private static void EnsureDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder(SYMBOLS_DIR))
            AssetDatabase.CreateFolder("Assets/Data", "Symbols");
        if (!AssetDatabase.IsValidFolder(PAYOUT_DIR))
            AssetDatabase.CreateFolder("Assets/Data", "Payout");
    }

    private static SymbolData CreateSymbol(string name, string spritePath, float weight, int payoutMultiplier)
    {
        string assetPath = $"{SYMBOLS_DIR}/{name}.asset";

        SymbolData existing = AssetDatabase.LoadAssetAtPath<SymbolData>(assetPath);
        if (existing != null)
        {
            Debug.Log($"Symbol asset already exists, skipping: {assetPath}");
            return existing;
        }

        SymbolData so = ScriptableObject.CreateInstance<SymbolData>();
        so.symbolName = name;
        so.dropWeight = weight;
        so.payoutMultiplier = payoutMultiplier;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite != null)
        {
            so.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite not found at {spritePath} for {name}");
        }

        AssetDatabase.CreateAsset(so, assetPath);
        Debug.Log($"Created: {assetPath} (weight={weight}, payout={payoutMultiplier}x)");
        return so;
    }

    private static void CreatePayoutTable(SymbolData s1, SymbolData s2, SymbolData s3, SymbolData s4)
    {
        string assetPath = $"{PAYOUT_DIR}/PayoutTable.asset";

        PayoutTable existing = AssetDatabase.LoadAssetAtPath<PayoutTable>(assetPath);
        if (existing != null)
        {
            Debug.Log($"PayoutTable asset already exists, skipping: {assetPath}");
            return;
        }

        PayoutTable so = ScriptableObject.CreateInstance<PayoutTable>();
        so.symbols = new SymbolData[] { s1, s2, s3, s4 };
        so.threeOfAKindMultipliers = new int[]
        {
            s1.payoutMultiplier,
            s2.payoutMultiplier,
            s3.payoutMultiplier,
            s4.payoutMultiplier
        };

        AssetDatabase.CreateAsset(so, assetPath);
        Debug.Log($"Created: {assetPath}");
    }
}
