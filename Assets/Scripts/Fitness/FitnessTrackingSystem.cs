using UnityEngine;
public class FitnessTrackingSystem : MonoBehaviour
{
    public static FitnessTrackingSystem Instance { get; private set; }
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;
    private float startTime;
    private bool isTracking;
    public float Duration => isTracking ? Time.time - startTime : 0;
    public float Calories => Duration * (difficultyScaler?.IntensityFactor ?? 1f) * 0.1f;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null) difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Running) { startTime = Time.time; isTracking = true; }
        else if (state == GameState.Dashboard) isTracking = false;
    }
}