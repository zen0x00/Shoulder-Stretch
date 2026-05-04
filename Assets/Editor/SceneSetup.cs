using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class SceneSetup
{
    [MenuItem("Tools/Shoulder Stretch/Setup Scene")]
    public static void SetupScene()
    {
        // --- MANAGERS ---
        GameObject managersRoot = new GameObject("_MANAGERS");

        GameObject gsObj = new GameObject("GameStateManager");
        gsObj.transform.SetParent(managersRoot.transform);
        GameStateManager gsm = gsObj.AddComponent<GameStateManager>();
        gsObj.AddComponent<DifficultyScaler>();

        GameObject fitnessObj = new GameObject("FitnessManager");
        fitnessObj.transform.SetParent(managersRoot.transform);
        FitnessTrackingSystem fitness = fitnessObj.AddComponent<FitnessTrackingSystem>();
        ScoringSystem scoring = fitnessObj.AddComponent<ScoringSystem>();
        fitnessObj.AddComponent<SafetySystem>();

        GameObject audioObj = new GameObject("AudioManager");
        audioObj.transform.SetParent(managersRoot.transform);
        AudioManager audioManager = audioObj.AddComponent<AudioManager>();
        audioObj.AddComponent<AudioSource>();

        // --- INPUT ---
        GameObject inputRoot = new GameObject("_INPUT");
        GameObject inputObj = new GameObject("InputSystem");
        inputObj.transform.SetParent(inputRoot.transform);
        InputSystem inputSystem = inputObj.AddComponent<InputSystem>();

        // --- ANALYTICS ---
        GameObject analyticsRoot = new GameObject("_ANALYTICS");
        GameObject analyticsObj = new GameObject("Analytics");
        analyticsObj.transform.SetParent(analyticsRoot.transform);
        analyticsObj.AddComponent<AnalyticsDashboardSystem>();

        // --- ENEMY SPAWNER ---
        GameObject spawnerObj = new GameObject("EnemySpawner");
        EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();

        // --- GROUND MANAGER ---
        GameObject groundManagerObj = new GameObject("GroundManager");
        GroundManager groundManager = groundManagerObj.AddComponent<GroundManager>();

        // Create placeholder ground prefab instance if none assigned
        GameObject groundPlaceholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        groundPlaceholder.name = "GroundPlaceholder";
        groundPlaceholder.transform.localScale = new Vector3(10f, 0.5f, 20f);
        groundPlaceholder.transform.position = new Vector3(0, -0.5f, 0);
        groundPlaceholder.transform.SetParent(groundManagerObj.transform);

        // --- PLAYER ---
        GameObject playerObj = new GameObject("Player");
        playerObj.transform.position = new Vector3(0, 0, 0);

        // Placeholder body (capsule)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Model";
        body.transform.SetParent(playerObj.transform);
        body.transform.localPosition = new Vector3(0, 1f, 0);
        body.transform.localScale = Vector3.one;

        // Aim points
        GameObject shootLeft = new GameObject("ShootLeft");
        shootLeft.transform.SetParent(playerObj.transform);
        shootLeft.transform.localPosition = new Vector3(-4f, 1f, 20f);

        GameObject shootRight = new GameObject("ShootRight");
        shootRight.transform.SetParent(playerObj.transform);
        shootRight.transform.localPosition = new Vector3(4f, 1f, 20f);

        // Barrel (muzzle tip)
        GameObject barrel = new GameObject("Barrel");
        barrel.transform.SetParent(playerObj.transform);
        barrel.transform.localPosition = new Vector3(0, 1.5f, 0.5f);

        // Bullet tracer
        GameObject tracerObj = new GameObject("BulletTracer");
        tracerObj.transform.SetParent(playerObj.transform);
        BulletTracer bulletTracer = tracerObj.AddComponent<BulletTracer>();
        LineRenderer lr = tracerObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0f;
        lr.enabled = false;

        // Muzzle flash
        GameObject muzzleFlash = new GameObject("MuzzleFlash");
        muzzleFlash.transform.SetParent(playerObj.transform);
        muzzleFlash.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
        ParticleSystem ps = muzzleFlash.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.startLifetime = 0.05f;
        main.startSize = 0.3f;
        main.startColor = Color.yellow;
        main.playOnAwake = false;

        // Player components
        PlayerController playerController = playerObj.AddComponent<PlayerController>();
        CombatSystem combatSystem = playerObj.AddComponent<CombatSystem>();
        Animator animator = playerObj.AddComponent<Animator>();

        // --- CAMERA ---
        Camera existingCam = Camera.main;
        GameObject camObj = existingCam != null ? existingCam.gameObject : new GameObject("Main Camera");
        if (existingCam == null) camObj.AddComponent<Camera>();
        camObj.tag = "MainCamera";
        camObj.transform.position = new Vector3(0, 5f, -8f);
        camObj.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
        CameraFollow cameraFollow = camObj.GetComponent<CameraFollow>() ?? camObj.AddComponent<CameraFollow>();

        // --- UI ---
        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
        GameObject canvasObj = existingCanvas != null ? existingCanvas.gameObject : new GameObject("Canvas");
        Canvas canvas = canvasObj.GetComponent<Canvas>() ?? canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if (canvasObj.GetComponent<CanvasScaler>() == null) canvasObj.AddComponent<CanvasScaler>();
        if (canvasObj.GetComponent<GraphicRaycaster>() == null) canvasObj.AddComponent<GraphicRaycaster>();

        UIManager uiManager = canvasObj.GetComponent<UIManager>() ?? canvasObj.AddComponent<UIManager>();

        // HUD Panel
        GameObject hudPanel = CreatePanel(canvasObj, "HUDPanel");
        HUDController hudController = hudPanel.AddComponent<HUDController>();

        TextMeshProUGUI scoreText    = CreateTMP(hudPanel, "ScoreText",      new Vector2(-300, 200));
        TextMeshProUGUI timerText    = CreateTMP(hudPanel, "TimerText",      new Vector2(0,    200));
        TextMeshProUGUI diffText     = CreateTMP(hudPanel, "DifficultyText", new Vector2(300,  200));
        TextMeshProUGUI ammoText     = CreateTMP(hudPanel, "AmmoText",       new Vector2(-300,-200));
        TextMeshProUGUI wavesText    = CreateTMP(hudPanel, "WavesText",      new Vector2(0,    0));

        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(hudPanel.transform, false);
        Slider healthSlider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchoredPosition = new Vector2(300, -200);
        sliderRect.sizeDelta = new Vector2(200, 30);

        // Other panels (placeholder — just empty panels)
        GameObject menuPanel          = CreatePanel(canvasObj, "MenuPanel");
        GameObject difficultyPanel    = CreatePanel(canvasObj, "DifficultyPanel");
        GameObject pausePanel         = CreatePanel(canvasObj, "PausePanel");
        GameObject gameOverPanel      = CreatePanel(canvasObj, "GameOverPanel");
        GameObject levelCompletePanel = CreatePanel(canvasObj, "LevelCompletePanel");
        GameObject dashboardPanel     = CreatePanel(canvasObj, "DashboardPanel");
        GameObject analyticsPanel     = CreatePanel(canvasObj, "AnalyticsPanel");
        analyticsPanel.AddComponent<ShowAnalytics>();
        GameObject graphPanel         = CreatePanel(canvasObj, "GraphPanel");
        graphPanel.AddComponent<GraphController>();
        GameObject damagePanel        = CreatePanel(canvasObj, "DamagePanel");
        Image dmgImage = damagePanel.AddComponent<Image>();
        dmgImage.color = new Color(1, 0, 0, 0.3f);

        GameObject sessionEndObj = new GameObject("SessionEnd");
        sessionEndObj.transform.SetParent(canvasObj.transform, false);
        sessionEndObj.AddComponent<SessionEndController>();

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // --- WIRE SERIALIZED FIELDS ---
        // GameStateManager
        SerializedObject gsmSO = new SerializedObject(gsm);
        gsmSO.FindProperty("playerController").objectReferenceValue = playerController;
        gsmSO.FindProperty("enemySpawner").objectReferenceValue = spawner;
        gsmSO.FindProperty("EnemySpawnerObj").objectReferenceValue = spawnerObj;
        gsmSO.ApplyModifiedProperties();

        // InputSystem
        SerializedObject inputSO = new SerializedObject(inputSystem);
        inputSO.FindProperty("animator").objectReferenceValue = animator;
        inputSO.ApplyModifiedProperties();

        // EnemySpawner
        SerializedObject spawnerSO = new SerializedObject(spawner);
        spawnerSO.FindProperty("audioManager").objectReferenceValue = audioManager;
        spawnerSO.FindProperty("wavesText").objectReferenceValue = wavesText;
        spawnerSO.ApplyModifiedProperties();

        // CombatSystem
        SerializedObject combatSO = new SerializedObject(combatSystem);
        combatSO.FindProperty("ShootLeftObj").objectReferenceValue = shootLeft.transform;
        combatSO.FindProperty("ShootRightObj").objectReferenceValue = shootRight.transform;
        combatSO.FindProperty("barrel").objectReferenceValue = barrel.transform;
        combatSO.FindProperty("bulletTracer").objectReferenceValue = bulletTracer;
        combatSO.FindProperty("muzzelFlash").objectReferenceValue = ps;
        combatSO.FindProperty("cameraFollow").objectReferenceValue = cameraFollow;
        combatSO.FindProperty("audioManager").objectReferenceValue = audioManager;
        combatSO.FindProperty("player").objectReferenceValue = playerController;
        combatSO.FindProperty("spawner").objectReferenceValue = spawner;
        combatSO.ApplyModifiedProperties();

        // PlayerController
        SerializedObject playerSO = new SerializedObject(playerController);
        playerSO.FindProperty("audioManager").objectReferenceValue = audioManager;
        playerSO.FindProperty("uiManager").objectReferenceValue = uiManager;
        playerSO.ApplyModifiedProperties();

        // HUDController
        SerializedObject hudSO = new SerializedObject(hudController);
        hudSO.FindProperty("scoreText").objectReferenceValue     = scoreText;
        hudSO.FindProperty("timerText").objectReferenceValue     = timerText;
        hudSO.FindProperty("difficultyText").objectReferenceValue = diffText;
        hudSO.FindProperty("ammoText").objectReferenceValue      = ammoText;
        hudSO.FindProperty("healthSlider").objectReferenceValue  = healthSlider;
        hudSO.FindProperty("scoring").objectReferenceValue       = scoring;
        hudSO.FindProperty("player").objectReferenceValue        = playerController;
        hudSO.FindProperty("fitness").objectReferenceValue       = fitness;
        hudSO.ApplyModifiedProperties();

        // UIManager panels
        SerializedObject uiSO = new SerializedObject(uiManager);
        uiSO.FindProperty("menuPanel").objectReferenceValue          = menuPanel;
        uiSO.FindProperty("difficultyPanel").objectReferenceValue    = difficultyPanel;
        uiSO.FindProperty("hudPanel").objectReferenceValue           = hudPanel;
        uiSO.FindProperty("pausePanel").objectReferenceValue         = pausePanel;
        uiSO.FindProperty("gameOverPanel").objectReferenceValue      = gameOverPanel;
        uiSO.FindProperty("levelCompletedPanel").objectReferenceValue = levelCompletePanel;
        uiSO.FindProperty("dashboardPanel").objectReferenceValue     = dashboardPanel;
        uiSO.FindProperty("analyticsPanel").objectReferenceValue     = analyticsPanel;
        uiSO.FindProperty("graphPanel").objectReferenceValue         = graphPanel;
        uiSO.FindProperty("damagePanel").objectReferenceValue        = damagePanel;
        uiSO.FindProperty("audioManager").objectReferenceValue       = audioManager;
        uiSO.ApplyModifiedProperties();

        Debug.Log("Scene setup complete. Assign: enemyPrefabs[] on EnemySpawner, AudioClips on AudioManager, UI Buttons on UIManager.");
        EditorUtility.DisplayDialog("Scene Setup", "Scene created!\n\nStill needed:\n• enemyPrefabs[] on EnemySpawner\n• AudioClips on AudioManager\n• UI Buttons on UIManager", "OK");
    }

    static GameObject CreatePanel(GameObject parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        panel.SetActive(false);
        return panel;
    }

    static TextMeshProUGUI CreateTMP(GameObject parent, string name, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);
        rt.anchoredPosition = anchoredPos;
        return tmp;
    }
}
