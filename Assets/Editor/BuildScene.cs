using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class BuildScene
{
    [MenuItem("Tools/Build Slot Machine Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Main Camera")
                Object.DestroyImmediate(root);
        }

        var cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.15f, 0.1f, 0.25f, 1f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0, 0, -10);

        // ── Canvas ──
        var canvasGO = CreateUIObject("Canvas", null);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1
            | AdditionalCanvasShaderChannels.Normal
            | AdditionalCanvasShaderChannels.Tangent;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Background ──
        var bgGO = CreateUIObject("Background", canvasGO.transform);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.sprite = LoadSprite("891afb2abfecfe94c8228786c61dd5da");
        bgImg.type = Image.Type.Simple;
        bgImg.preserveAspect = false;
        StretchFill(bgGO.GetComponent<RectTransform>());

        // ── MachineContainer ──
        var machineContainer = CreateUIObject("MachineContainer", canvasGO.transform);
        var mcRect = machineContainer.GetComponent<RectTransform>();
        mcRect.anchorMin = new Vector2(0.5f, 0.5f);
        mcRect.anchorMax = new Vector2(0.5f, 0.5f);
        mcRect.pivot = new Vector2(0.5f, 0.5f);
        mcRect.sizeDelta = new Vector2(650f, 650f);
        mcRect.anchoredPosition = Vector2.zero;

        // MachineFrame
        var frameGO = CreateUIObject("MachineFrame", machineContainer.transform);
        var frameImg = frameGO.AddComponent<Image>();
        frameImg.sprite = LoadSprite("0c2458e564b04de40b66207584cc2f17");
        frameImg.type = Image.Type.Simple;
        frameImg.preserveAspect = true;
        StretchFill(frameGO.GetComponent<RectTransform>());

        // ── Reel Masks ──
        // Manually tuned local positions inside the 650x650 MachineContainer.
        Vector3[] reelMaskPos = {
            new Vector3(-170f, -30f, 0f),
            new Vector3(-30f, -30f, 0f),
            new Vector3(105f, -30f, 0f)
        };
        float maskW = 110f;
        float maskH = 160f;
        var reelMaskGOs = new GameObject[3];
        var reelStripGOs = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            var maskGO = CreateUIObject($"ReelMask_{i}", machineContainer.transform);
            var maskRect = maskGO.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0.5f, 0.5f);
            maskRect.anchorMax = new Vector2(0.5f, 0.5f);
            maskRect.pivot = new Vector2(0.5f, 0.5f);
            maskRect.sizeDelta = new Vector2(maskW, maskH);
            maskRect.anchoredPosition = reelMaskPos[i];
            maskGO.AddComponent<RectMask2D>();
            reelMaskGOs[i] = maskGO;

            var stripGO = CreateUIObject($"ReelStripContent_{i}", maskGO.transform);
            var stripRect = stripGO.GetComponent<RectTransform>();
            stripRect.anchorMin = new Vector2(0.5f, 1f);
            stripRect.anchorMax = new Vector2(0.5f, 1f);
            stripRect.pivot = new Vector2(0.5f, 1f);
            stripRect.sizeDelta = new Vector2(100, 1100);
            stripRect.anchoredPosition = Vector2.zero;
            reelStripGOs[i] = stripGO;
        }

        // ── UIPanel ──
        var uiPanel = CreateUIObject("UIPanel", canvasGO.transform);
        var uiPanelRect = uiPanel.GetComponent<RectTransform>();
        uiPanelRect.anchorMin = new Vector2(0f, 0f);
        uiPanelRect.anchorMax = new Vector2(1f, 0f);
        uiPanelRect.pivot = new Vector2(0.5f, 0f);
        uiPanelRect.sizeDelta = new Vector2(0, 80);
        uiPanelRect.anchoredPosition = Vector2.zero;

        // ── Bottom UI — manually tuned positions ──
        CreateLabel(uiPanel.transform, "CreditsLabel", "CREDITS", new Vector2(-400, 85), 20);
        CreateValueText(uiPanel.transform, "CreditsText", "1000", new Vector2(-400, 50), 28);
        CreateLabel(uiPanel.transform, "BetLabel", "BET", new Vector2(0, 85), 20);
        CreateValueText(uiPanel.transform, "BetText", "10", new Vector2(0, 50), 28);
        CreateLabel(uiPanel.transform, "WinLabel", "WIN", new Vector2(400, 85), 20);
        CreateValueText(uiPanel.transform, "WinText", "0", new Vector2(400, 50), 28);

        // ── LeverVisual ──
        // Manually tuned position inside the 650x650 MachineContainer.
        var leverVisualGO = CreateUIObject("LeverVisual", machineContainer.transform);
        var leverVisRect = leverVisualGO.GetComponent<RectTransform>();
        leverVisRect.anchorMin = new Vector2(0.5f, 0.5f);
        leverVisRect.anchorMax = new Vector2(0.5f, 0.5f);
        leverVisRect.pivot = new Vector2(0.5f, 0.5f);
        leverVisRect.sizeDelta = new Vector2(94f, 272f);
        leverVisRect.anchoredPosition = new Vector2(245f, -275f);
        var leverImg = leverVisualGO.AddComponent<Image>();
        leverImg.sprite = LoadSprite("f67aab7c3b90a0147bbea5420c271311");
        leverImg.preserveAspect = true;
        leverImg.raycastTarget = false;

        // ── LeverHitArea: transparent Button over the lever ──
        // Manually tuned size/position aligned with LeverVisual.
        var leverHitGO = CreateUIObject("LeverHitArea", machineContainer.transform);
        var leverHitRect = leverHitGO.GetComponent<RectTransform>();
        leverHitRect.anchorMin = new Vector2(0.5f, 0.5f);
        leverHitRect.anchorMax = new Vector2(0.5f, 0.5f);
        leverHitRect.pivot = new Vector2(0.5f, 0.5f);
        leverHitRect.sizeDelta = new Vector2(90f, 230f);
        leverHitRect.anchoredPosition = new Vector2(245f, -275f);
        var leverHitImg = leverHitGO.AddComponent<Image>();
        leverHitImg.color = new Color(0, 0, 0, 0);
        leverHitGO.AddComponent<Button>();

        // ── WinPopup (inactive) ──
        var popupGO = CreateUIObject("WinPopup", canvasGO.transform);
        popupGO.SetActive(false);
        var popupRect = popupGO.GetComponent<RectTransform>();
        StretchFill(popupRect);

        var popupOverlay = CreateUIObject("PopupOverlay", popupGO.transform);
        var popupOverlayImg = popupOverlay.AddComponent<Image>();
        popupOverlayImg.color = new Color(0, 0, 0, 0.5f);
        StretchFill(popupOverlay.GetComponent<RectTransform>());

        var popupPanel = CreateUIObject("PopupPanel", popupGO.transform);
        var popupPanelImg = popupPanel.AddComponent<Image>();
        popupPanelImg.sprite = LoadSprite("22515f8310483534fa4e0fc29072c070");
        popupPanelImg.type = Image.Type.Simple;
        popupPanelImg.preserveAspect = false;
        var ppRect = popupPanel.GetComponent<RectTransform>();
        ppRect.anchorMin = new Vector2(0.5f, 0.5f);
        ppRect.anchorMax = new Vector2(0.5f, 0.5f);
        ppRect.pivot = new Vector2(0.5f, 0.5f);
        ppRect.sizeDelta = new Vector2(800, 400);
        ppRect.anchoredPosition = new Vector2(0, 50);

        var winAmtGO = CreateUIObject("WinAmountText", popupPanel.transform);
        var winAmtRect = winAmtGO.GetComponent<RectTransform>();
        winAmtRect.anchorMin = new Vector2(0.5f, 1f);
        winAmtRect.anchorMax = new Vector2(0.5f, 1f);
        winAmtRect.pivot = new Vector2(0.5f, 1f);
        winAmtRect.sizeDelta = new Vector2(600, 80);
        winAmtRect.anchoredPosition = new Vector2(0, -30);
        var winAmtTMP = winAmtGO.AddComponent<TextMeshProUGUI>();
        winAmtTMP.text = "";
        winAmtTMP.fontSize = 48;
        winAmtTMP.alignment = TextAlignmentOptions.Center;
        winAmtTMP.color = Color.white;

        var yesBtnGO = CreateUIObject("YesButton", popupPanel.transform);
        var yesBtnRect = yesBtnGO.GetComponent<RectTransform>();
        yesBtnRect.anchorMin = new Vector2(0.3f, 0f);
        yesBtnRect.anchorMax = new Vector2(0.3f, 0f);
        yesBtnRect.pivot = new Vector2(0.5f, 0f);
        yesBtnRect.sizeDelta = new Vector2(200, 60);
        yesBtnRect.anchoredPosition = new Vector2(0, 20);
        var yesBtnImg = yesBtnGO.AddComponent<Image>();
        yesBtnImg.sprite = LoadSprite("eeb053c62cfc19d4eac8d4988d4dac5f");
        yesBtnImg.type = Image.Type.Simple;
        yesBtnImg.preserveAspect = true;
        yesBtnGO.AddComponent<Button>();

        var noBtnGO = CreateUIObject("NoButton", popupPanel.transform);
        var noBtnRect = noBtnGO.GetComponent<RectTransform>();
        noBtnRect.anchorMin = new Vector2(0.7f, 0f);
        noBtnRect.anchorMax = new Vector2(0.7f, 0f);
        noBtnRect.pivot = new Vector2(0.5f, 0f);
        noBtnRect.sizeDelta = new Vector2(200, 60);
        noBtnRect.anchoredPosition = new Vector2(0, 20);
        var noBtnImg = noBtnGO.AddComponent<Image>();
        noBtnImg.sprite = LoadSprite("eeb053c62cfc19d4eac8d4988d4dac5f");
        noBtnImg.type = Image.Type.Simple;
        noBtnImg.preserveAspect = true;
        noBtnGO.AddComponent<Button>();

        // ── WinPopup wiring (must happen before GameManager references it) ──
        var popup = popupGO.AddComponent<WinPopup>();
        var wpSO = new SerializedObject(popup);
        wpSO.FindProperty("popupPanel").objectReferenceValue = popupPanel;
        wpSO.FindProperty("winAmountText").objectReferenceValue = winAmtTMP;
        wpSO.FindProperty("yesButton").objectReferenceValue = yesBtnGO.GetComponent<Button>();
        wpSO.FindProperty("noButton").objectReferenceValue = noBtnGO.GetComponent<Button>();
        wpSO.ApplyModifiedProperties();

        // ── Wallet ──
        var walletGO = new GameObject("Wallet");
        var wallet = walletGO.AddComponent<Wallet>();

        // ── UIManager ──
        var uiManagerGO = new GameObject("UIManager");
        var uiMgr = uiManagerGO.AddComponent<UIManager>();
        var uiMgrSO = new SerializedObject(uiMgr);
        uiMgrSO.FindProperty("wallet").objectReferenceValue = wallet;
        uiMgrSO.FindProperty("creditsText").objectReferenceValue = FindChildTMP(uiPanel, "CreditsText");
        uiMgrSO.FindProperty("betText").objectReferenceValue = FindChildTMP(uiPanel, "BetText");
        uiMgrSO.FindProperty("winText").objectReferenceValue = FindChildTMP(uiPanel, "WinText");
        uiMgrSO.ApplyModifiedProperties();

        // ── ReelColumn on each ReelMask ──
        SymbolData[] allSymbols = LoadSymbolAssets();
        for (int i = 0; i < 3; i++)
        {
            var reelCol = reelMaskGOs[i].AddComponent<ReelColumn>();
            var rcSO = new SerializedObject(reelCol);
            rcSO.FindProperty("allSymbols").arraySize = allSymbols.Length;
            for (int s = 0; s < allSymbols.Length; s++)
                rcSO.FindProperty("allSymbols").GetArrayElementAtIndex(s).objectReferenceValue = allSymbols[s];
            rcSO.FindProperty("reelStripContent").objectReferenceValue = reelStripGOs[i].GetComponent<RectTransform>();
            rcSO.FindProperty("symbolHeight").floatValue = 100f;
            rcSO.FindProperty("paddingCount").intValue = 8;
            rcSO.ApplyModifiedProperties();
        }

        // ── ReelManager ──
        var rmGO = new GameObject("ReelManager");
        var reelMgr = rmGO.AddComponent<ReelManager>();
        var rmSO = new SerializedObject(reelMgr);
        rmSO.FindProperty("reels").arraySize = 3;
        for (int i = 0; i < 3; i++)
            rmSO.FindProperty("reels").GetArrayElementAtIndex(i).objectReferenceValue = reelMaskGOs[i].GetComponent<ReelColumn>();
        rmSO.ApplyModifiedProperties();

        // ── LeverController ──
        var lcGO = new GameObject("LeverController");
        var leverCtrl = lcGO.AddComponent<LeverController>();
        var lcSO = new SerializedObject(leverCtrl);
        lcSO.FindProperty("leverImage").objectReferenceValue = leverImg;
        lcSO.FindProperty("leverButton").objectReferenceValue = leverHitGO.GetComponent<Button>();
        lcSO.FindProperty("leverUp").objectReferenceValue = leverImg.sprite;
        lcSO.FindProperty("leverDown").objectReferenceValue = LoadSprite("546541a6bbeef114abcc35cb39cd0fe6");
        lcSO.ApplyModifiedProperties();

        // ── GameManager ──
        PayoutTable payoutTable = LoadPayoutTable();
        if (payoutTable == null)
        {
            Debug.LogError("BuildScene aborted: PayoutTable is required but missing.");
            return;
        }
        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        var gmSO = new SerializedObject(gm);
        gmSO.FindProperty("wallet").objectReferenceValue = wallet;
        gmSO.FindProperty("reelManager").objectReferenceValue = reelMgr;
        gmSO.FindProperty("payoutTable").objectReferenceValue = payoutTable;
        gmSO.FindProperty("uiManager").objectReferenceValue = uiMgr;
        gmSO.FindProperty("winPopup").objectReferenceValue = popup;
        gmSO.FindProperty("leverController").objectReferenceValue = leverCtrl;
        gmSO.FindProperty("symbols").arraySize = allSymbols.Length;
        for (int i = 0; i < allSymbols.Length; i++)
            gmSO.FindProperty("symbols").GetArrayElementAtIndex(i).objectReferenceValue = allSymbols[i];
        gmSO.ApplyModifiedProperties();

        // Now that GameManager exists, wire it into LeverController
        lcSO = new SerializedObject(leverCtrl);
        lcSO.FindProperty("gameManager").objectReferenceValue = gm;
        lcSO.ApplyModifiedProperties();

        // ── SlotAudioController ──
        var audioGO = new GameObject("SlotAudioController");
        var audioSrc = audioGO.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.spatialBlend = 0f;
        audioSrc.spread = 0f;
        var audioCtrl = audioGO.AddComponent<SlotAudioController>();
        var acSO = new SerializedObject(audioCtrl);
        acSO.FindProperty("slotMachineClip").objectReferenceValue = LoadAudioClip("c0a0b33d1fe74404c96e32e2c50dda83");
        acSO.FindProperty("leverClip").objectReferenceValue = LoadAudioClip("9b8725246303d814fbbb27c53322a419");
        acSO.FindProperty("winJackpotClip").objectReferenceValue = LoadAudioClip("d855d56a50752f24d95f8cd8efbb3984");
        acSO.ApplyModifiedProperties();

        // Wire SlotAudioController into GameManager
        gmSO = new SerializedObject(gm);
        gmSO.FindProperty("audioController").objectReferenceValue = audioCtrl;
        gmSO.ApplyModifiedProperties();

        // Wire SlotAudioController into LeverController
        lcSO = new SerializedObject(leverCtrl);
        lcSO.FindProperty("audioController").objectReferenceValue = audioCtrl;
        lcSO.ApplyModifiedProperties();

        // ── EventSystem ──
        var existingES = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        GameObject esGO;
        if (existingES != null)
        {
            esGO = existingES.gameObject;
            var oldModule = existingES.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (oldModule != null)
                Object.DestroyImmediate(oldModule);
            if (existingES.GetComponent<InputSystemUIInputModule>() == null)
                esGO.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<InputSystemUIInputModule>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Slot machine scene built successfully with all references wired.");
    }

    static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        if (parent != null)
            go.transform.SetParent(parent, false);
        return go;
    }

    static void StretchFill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    static void CreateLabel(Transform parent, string name, string text, Vector2 pos, float fontSize)
    {
        var go = CreateUIObject(name, parent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 30);
        rt.anchoredPosition = pos;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    static void CreateValueText(Transform parent, string name, string text, Vector2 pos, float fontSize)
    {
        var go = CreateUIObject(name, parent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 40);
        rt.anchoredPosition = pos;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
    }

    static TextMeshProUGUI FindChildTMP(GameObject parent, string childName)
    {
        var transform = parent.transform.Find(childName);
        if (transform != null)
            return transform.GetComponent<TextMeshProUGUI>();
        return null;
    }

    static SymbolData[] LoadSymbolAssets()
    {
        string[] paths = new string[]
        {
            "Assets/Prefabs/Symbols/Symbol_1.asset",
            "Assets/Prefabs/Symbols/Symbol_2.asset",
            "Assets/Prefabs/Symbols/Symbol_3.asset",
            "Assets/Prefabs/Symbols/Symbol_4.asset",
        };

        SymbolData[] assets = new SymbolData[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<SymbolData>(paths[i]);
            if (assets[i] == null)
                Debug.LogWarning($"BuildScene: SymbolData not found at {paths[i]}");
        }
        return assets;
    }

    // PayoutTable is a required dependency for GameManager — it determines win/loss
    // on every completed spin. A missing PayoutTable causes a NullReferenceException
    // in OnAllReelsStopped, so we fail loudly rather than silently assign null.
    static PayoutTable LoadPayoutTable()
    {
        const string path = "Assets/Prefabs/Payout/PayoutTable.asset";
        PayoutTable pt = AssetDatabase.LoadAssetAtPath<PayoutTable>(path);
        if (pt == null)
            Debug.LogError($"BuildScene: PayoutTable asset not found at {path}. " +
                "Run Tools > Create Slot Machine Assets first, then rebuild the scene.");
        return pt;
    }

    static AudioClip LoadAudioClip(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
    }

    static Sprite LoadSprite(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
