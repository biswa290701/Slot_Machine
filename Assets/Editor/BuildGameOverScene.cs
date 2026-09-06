using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class BuildGameOverScene
{
    const string ScenePath = "Assets/Scenes/GameOver.unity";

    [MenuItem("Tools/Build Game Over UI")]
    public static void Run()
    {
        if (!System.IO.File.Exists(ScenePath))
        {
            Debug.LogError("BuildGameOverScene: GameOver.unity not found at " + ScenePath);
            return;
        }

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        // ── Canvas ──
        var canvasGO = FindOrCreate("Canvas");
        var canvas = canvasGO.GetComponent<Canvas>();
        if (canvas == null) canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1
            | AdditionalCanvasShaderChannels.Normal
            | AdditionalCanvasShaderChannels.Tangent;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        if (canvasGO.GetComponent<GraphicRaycaster>() == null)
            canvasGO.AddComponent<GraphicRaycaster>();

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

        // ── Background safety ──
        var bgGO = GameObject.Find("Background");
        if (bgGO != null)
        {
            var bgImg = bgGO.GetComponent<Image>();
            if (bgImg != null) bgImg.raycastTarget = false;
        }

        var canvasTransform = canvasGO.transform;

        // ── Game Over text ──
        var gameOverGO = FindOrCreateChild(canvasTransform, "GameOverText");
        var gameOverRect = EnsureRectTransform(gameOverGO);
        gameOverRect.anchorMin = new Vector2(0.5f, 1f);
        gameOverRect.anchorMax = new Vector2(0.5f, 1f);
        gameOverRect.pivot = new Vector2(0.5f, 1f);
        gameOverRect.sizeDelta = new Vector2(800, 120);
        gameOverRect.anchoredPosition = new Vector2(0f, -150f);

        var gameOverTMP = gameOverGO.GetComponent<TextMeshProUGUI>();
        if (gameOverTMP == null) gameOverTMP = gameOverGO.AddComponent<TextMeshProUGUI>();
        gameOverTMP.text = "GAME OVER";
        gameOverTMP.fontSize = 96;
        gameOverTMP.fontStyle = FontStyles.Bold;
        gameOverTMP.alignment = TextAlignmentOptions.Center;
        gameOverTMP.color = Color.white;
        gameOverTMP.raycastTarget = false;

        // ── Restart button ──
        var restartBtnGO = FindOrCreateChild(canvasTransform, "RestartButton");
        var restartBtnRect = EnsureRectTransform(restartBtnGO);
        restartBtnRect.anchorMin = new Vector2(0.5f, 1f);
        restartBtnRect.anchorMax = new Vector2(0.5f, 1f);
        restartBtnRect.pivot = new Vector2(0.5f, 1f);
        restartBtnRect.sizeDelta = new Vector2(260, 70);
        restartBtnRect.anchoredPosition = new Vector2(0f, -320f);

        var restartBtnImg = restartBtnGO.GetComponent<Image>();
        if (restartBtnImg == null) restartBtnImg = restartBtnGO.AddComponent<Image>();
        restartBtnImg.color = new Color(0.25f, 0.25f, 0.35f, 1f);
        restartBtnImg.raycastTarget = true;

        var restartBtn = restartBtnGO.GetComponent<Button>();
        if (restartBtn == null) restartBtn = restartBtnGO.AddComponent<Button>();
        restartBtn.interactable = true;
        restartBtn.targetGraphic = restartBtnImg;
        restartBtn.navigation = new Navigation { mode = Navigation.Mode.Explicit };
        var colors = restartBtn.colors;
        colors.normalColor = new Color(0.25f, 0.25f, 0.35f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.55f, 1f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.25f, 1f);
        colors.selectedColor = new Color(0.85f, 0.65f, 0.15f, 1f);
        colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        colors.fadeDuration = 0.1f;
        restartBtn.colors = colors;

        // Button label
        var labelGO = FindOrCreateChild(restartBtnGO.transform, "Text");
        var labelRect = EnsureRectTransform(labelGO);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;
        labelRect.anchoredPosition = Vector2.zero;

        var labelTMP = labelGO.GetComponent<TextMeshProUGUI>();
        if (labelTMP == null) labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = "RESTART";
        labelTMP.fontSize = 40;
        labelTMP.fontStyle = FontStyles.Bold;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.color = Color.white;
        labelTMP.raycastTarget = false;

        // ── GameOverController + onClick wiring ──
        var ctrlGO = FindOrCreate("GameOverController");
        var ctrl = ctrlGO.GetComponent<GameOverController>();
        if (ctrl == null) ctrl = ctrlGO.AddComponent<GameOverController>();

        // AudioSource for game_over sound
        var audioSrc = ctrlGO.GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = ctrlGO.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        audioSrc.spatialBlend = 0f;
        audioSrc.volume = 1f;

        // Wire game_over AudioClip via SerializedObject
        var gameOverClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
            AssetDatabase.GUIDToAssetPath("212a298860beec14eb2a73b6f14bb23f"));
        if (gameOverClip != null)
        {
            var ctrlSO = new SerializedObject(ctrl);
            ctrlSO.FindProperty("gameOverClip").objectReferenceValue = gameOverClip;
            ctrlSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("BuildGameOverScene: game_over audio clip not found.");
        }

        restartBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.AddListener(ctrl.RestartGame);

        // ── Save ──
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game Over UI built and saved.");
    }

    static GameObject FindOrCreate(string name)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name);
            if (EditorSceneManager.GetActiveScene().isLoaded)
                Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        }
        return go;
    }

    static GameObject FindOrCreateChild(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static RectTransform EnsureRectTransform(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null)
        {
            go.AddComponent<RectTransform>();
            rt = go.GetComponent<RectTransform>();
        }
        return rt;
    }
}
