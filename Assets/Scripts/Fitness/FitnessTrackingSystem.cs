using UnityEngine;
public class FitnessTrackingSystem : MonoBehaviour
{
    public static FitnessTrackingSystem Instance { get; private set; }
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;
    [SerializeField] private float sessionTimeLimit = 120f;
    private float startTime = 0f;
    private float cachedDuration;
    private bool isTracking;
    public float Duration => isTracking ? Time.time - startTime : cachedDuration;
    public float TimeRemaining => isTracking ? sessionTimeLimit - (Time.time - startTime): 0;
    public float Calories => Duration * (difficultyScaler?.IntensityFactor ?? 1f) * 0.1f;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        startTime = Time.time;
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null) difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void Update()
    {
        if (!isTracking) return;

        if(Duration >= sessionTimeLimit)
        {
            cachedDuration = Time.time - startTime;
            isTracking = false;
            gameManager?.EndGame();
        }
    }

    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Running) { startTime = Time.time; isTracking = true; }
        else if (state == GameState.Dashboard) isTracking = false;
    }
}