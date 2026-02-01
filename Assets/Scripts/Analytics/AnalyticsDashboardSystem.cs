using UnityEngine;
using UnityEngine.UI;
public class AnalyticsDashboardSystem : MonoBehaviour
{
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private FitnessTrackingSystem fitnessSystem;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private Text durationText, calText, accuracyText, scoreText, highScoreText;
    [SerializeField] private Button playAgainBtn, mainMenuBtn;
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (fitnessSystem == null) fitnessSystem = FindFirstObjectByType<FitnessTrackingSystem>();
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (scoringSystem == null) scoringSystem = FindFirstObjectByType<ScoringSystem>();
        if (playAgainBtn) playAgainBtn.onClick.AddListener(() => gameManager?.RestartGame());
        if (mainMenuBtn) mainMenuBtn.onClick.AddListener(() => gameManager?.ReturnToIdle());
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void HandleStateChange(GameState state) { if (state == GameState.Dashboard) Populate(); }
    private void Populate()
    {
        if (fitnessSystem)
        {
            durationText.text = $"Duration: {System.TimeSpan.FromSeconds(fitnessSystem.Duration):mm\\:ss}";
            calText.text = $"Calories: {fitnessSystem.Calories:F1}";
        }
        if (inputSystem) accuracyText.text = $"Accuracy: {inputSystem.AccuracyPercentage:F0}%";
        if (scoringSystem)
        {
            scoreText.text = $"Final Score: {scoringSystem.CurrentScore}";
            highScoreText.text = $"High Score: {scoringSystem.HighScore}";
        }
    }
}