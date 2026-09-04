using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class BuildScene
{
    [MenuItem("Tools/Build Slot Machine Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // Clear existing root objects except camera
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Main Camera")
                Object.DestroyImmediate(root);
        }

        // Configure Main Camera
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
        mcRect.sizeDelta = new Vector2(650, 650);
        mcRect.anchoredPosition = Vector2.zero;

        // MachineFrame
        var frameGO = CreateUIObject("MachineFrame", machineContainer.transform);
        var frameImg = frameGO.AddComponent<Image>();
        frameImg.sprite = LoadSprite("0c2458e564b04de40b66207584cc2f17");
        frameImg.type = Image.Type.Simple;
        frameImg.preserveAspect = true;
        StretchFill(frameGO.GetComponent<RectTransform>());

        // ── Reel Masks ──
        float[] reelX = { -120f, 0f, 120f };
        float reelY = 40f;
        float maskW = 110f;
        float maskH = 160f;

        for (int i = 0; i < 3; i++)
        {
            var maskGO = CreateUIObject($"ReelMask_{i}", machineContainer.transform);
            var maskRect = maskGO.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0.5f, 0.5f);
            maskRect.anchorMax = new Vector2(0.5f, 0.5f);
            maskRect.pivot = new Vector2(0.5f, 0.5f);
            maskRect.sizeDelta = new Vector2(maskW, maskH);
            maskRect.anchoredPosition = new Vector2(reelX[i], reelY);
            maskGO.AddComponent<RectMask2D>();

            var stripGO = CreateUIObject($"ReelStripContent_{i}", maskGO.transform);
            var stripRect = stripGO.GetComponent<RectTransform>();
            stripRect.anchorMin = new Vector2(0.5f, 1f);
            stripRect.anchorMax = new Vector2(0.5f, 1f);
            stripRect.pivot = new Vector2(0.5f, 1f);
            stripRect.sizeDelta = new Vector2(100, 1100);
            stripRect.anchoredPosition = Vector2.zero;
        }

        // ── UIPanel ──
        var uiPanel = CreateUIObject("UIPanel", canvasGO.transform);
        var uiPanelRect = uiPanel.GetComponent<RectTransform>();
        uiPanelRect.anchorMin = new Vector2(0f, 0f);
        uiPanelRect.anchorMax = new Vector2(1f, 0f);
        uiPanelRect.pivot = new Vector2(0.5f, 0f);
        uiPanelRect.sizeDelta = new Vector2(0, 80);
        uiPanelRect.anchoredPosition = Vector2.zero;

        // Credits
        CreateLabel(uiPanel.transform, "CreditsLabel", "CREDITS", new Vector2(-500, 45), 20);
        CreateValueText(uiPanel.transform, "CreditsText", "1000", new Vector2(-500, 10), 28);

        // Bet
        CreateLabel(uiPanel.transform, "BetLabel", "BET", new Vector2(-100, 45), 20);
        CreateValueText(uiPanel.transform, "BetText", "10", new Vector2(-100, 10), 28);

        // Win
        CreateLabel(uiPanel.transform, "WinLabel", "WIN", new Vector2(300, 45), 20);
        CreateValueText(uiPanel.transform, "WinText", "", new Vector2(300, 10), 28);

        // ── Lever ──
        var leverGO = CreateUIObject("Lever", machineContainer.transform);
        var leverRect = leverGO.GetComponent<RectTransform>();
        leverRect.anchorMin = new Vector2(1f, 0.5f);
        leverRect.anchorMax = new Vector2(1f, 0.5f);
        leverRect.pivot = new Vector2(0.5f, 0f);
        leverRect.sizeDelta = new Vector2(94, 272);
        leverRect.anchoredPosition = new Vector2(10, -60);
        var leverImg = leverGO.AddComponent<Image>();
        leverImg.sprite = LoadSprite("f67aab7c3b90a0147bbea5420c271311");
        leverImg.raycastTarget = true;
        leverGO.AddComponent<Button>();

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
        popupPanelImg.sprite = LoadSprite("c95d78159310e6d4495736826ae2b924");
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

        // ── GameManager ──
        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();

        // ── ReelManager ──
        var rmGO = new GameObject("ReelManager");
        rmGO.AddComponent<ReelManager>();

        // ── LeverController ──
        var lcGO = new GameObject("LeverController");
        var leverCtrl = lcGO.AddComponent<LeverController>();

        // Wire LeverController serialized references via SerializedObject
        var lcSO = new SerializedObject(leverCtrl);
        lcSO.FindProperty("gameManager").objectReferenceValue = gm;
        lcSO.FindProperty("leverImage").objectReferenceValue = leverImg;
        lcSO.FindProperty("leverButton").objectReferenceValue = leverGO.GetComponent<Button>();
        lcSO.FindProperty("leverUp").objectReferenceValue = leverImg.sprite;
        lcSO.FindProperty("leverDown").objectReferenceValue = LoadSprite("546541a6bbeef114abcc35cb39cd0fe6");
        lcSO.ApplyModifiedProperties();

        // ── EventSystem ──
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Slot machine scene built successfully. Wire Inspector references on GameManager, ReelManager, UIManager, and Wallet.");
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

    static Sprite LoadSprite(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
