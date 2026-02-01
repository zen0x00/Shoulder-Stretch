using UnityEngine;
using System;
public class ScoringSystem : MonoBehaviour
{
    public static ScoringSystem Instance { get; private set; }
    public event Action<int> OnScoreChanged;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private int basePoints = 10;
    private int currentScore;
    private int highScore;
    private int combo;
    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    public int Combo => combo;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (inputSystem != null) inputSystem.OnActionPerformed += HandleAction;
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void OnDestroy()
    {
        if (inputSystem != null) inputSystem.OnActionPerformed -= HandleAction;
        if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange;
    }
    private void HandleAction(ActionType action, bool success)
    {
        if (success) { combo++; currentScore += basePoints * combo; OnScoreChanged?.Invoke(currentScore); }
        else combo = 0;
    }
    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Running) { currentScore = 0; combo = 0; OnScoreChanged?.Invoke(0); }
        else if (state == GameState.Dashboard) SaveScore();
    }
    private void SaveScore() { if (currentScore > highScore) { highScore = currentScore; PlayerPrefs.SetInt("HighScore", highScore); } }
}