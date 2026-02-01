using UnityEngine;
public class DifficultyScaler : MonoBehaviour
{
    public static DifficultyScaler Instance { get; private set; }
    [SerializeField] private GameStateManager gameManager;
    [System.Serializable]
    public class DifficultySettings
    {
        public float forwardSpeed = 5f;
        public float horizontalSpeed = 5f;
        public float boundaryX = 4f;
        public float enemySpawnInterval = 3f;
        public int maxEnemies = 5;
        public float intensityFactor = 1f;
        public float cooldownMultiplier = 1f;
        public int initialAmmo = 5;
    }
    [SerializeField] private DifficultySettings beginner = new DifficultySettings { forwardSpeed = 4f, horizontalSpeed = 4f, boundaryX = 3f, enemySpawnInterval = 4f, maxEnemies = 3, intensityFactor = 0.5f, cooldownMultiplier = 0.8f, initialAmmo = 10 };
    [SerializeField] private DifficultySettings moderate = new DifficultySettings { forwardSpeed = 6f, horizontalSpeed = 6f, boundaryX = 4f, enemySpawnInterval = 2.5f, maxEnemies = 5, intensityFactor = 1.0f, cooldownMultiplier = 1.0f, initialAmmo = 5 };
    [SerializeField] private DifficultySettings expert = new DifficultySettings { forwardSpeed = 8f, horizontalSpeed = 8f, boundaryX = 5f, enemySpawnInterval = 1.5f, maxEnemies = 8, intensityFactor = 1.5f, cooldownMultiplier = 1.2f, initialAmmo = 3 };
    private DifficultySettings currentSettings;
    public float ForwardSpeed => currentSettings?.forwardSpeed ?? 5f;
    public float HorizontalSpeed => currentSettings?.horizontalSpeed ?? 5f;
    public float BoundaryX => currentSettings?.boundaryX ?? 4f;
    public float EnemySpawnInterval => currentSettings?.enemySpawnInterval ?? 3f;
    public int MaxEnemies => currentSettings?.maxEnemies ?? 5;
    public float IntensityFactor => currentSettings?.intensityFactor ?? 1f;
    public float CooldownMultiplier => currentSettings?.cooldownMultiplier ?? 1f;
    public int InitialAmmo => currentSettings?.initialAmmo ?? 5;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        currentSettings = moderate;
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (gameManager != null) gameManager.OnDifficultyChanged += HandleDifficultyChange;
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnDifficultyChanged -= HandleDifficultyChange; }
    private void HandleDifficultyChange(Difficulty difficulty)
    {
        currentSettings = difficulty switch
        {
            Difficulty.Beginner => beginner,
            Difficulty.Moderate => moderate,
            Difficulty.Expert => expert,
            _ => moderate
        };
    }
}