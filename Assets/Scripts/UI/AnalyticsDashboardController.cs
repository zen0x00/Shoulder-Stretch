using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class AnalyticsDashboardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText, leftText, rightText, totalText, accuracyText, difficultyText, calorieText, scoreText;
    [SerializeField] private Button returnMenuBtn;
    [SerializeField] private ScoringSystem scoring;
    [SerializeField] private FitnessTrackingSystem fitness;
    [SerializeField] private InputSystem input;
    [SerializeField] private GameStateManager gameManager;
    private void OnEnable() { if (gameManager != null && gameManager.CurrentState == GameState.Dashboard) Populate(); }
    private void Start()
    {
        if (scoring == null) scoring = FindFirstObjectByType<ScoringSystem>();
        if (fitness == null) fitness = FindFirstObjectByType<FitnessTrackingSystem>();
        if (input == null) input = FindFirstObjectByType<InputSystem>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (returnMenuBtn) returnMenuBtn.onClick.AddListener(() => gameManager?.ReturnToIdle());
    }
    public void Populate()
    {
        if (fitness)
        {
            TimeSpan t = TimeSpan.FromSeconds(fitness.Duration);
            timeText.text = $"Time Played: {t.Minutes:D2}:{t.Seconds:D2}";
            calorieText.text = $"Calories Burned: {fitness.Calories:F1}";
        }
        if (input)
        {
            leftText.text = $"Left Actions: {input.LeftActionCount}";
            rightText.text = $"Right Actions: {input.RightActionCount}";
            totalText.text = $"Total Actions: {input.TotalActions}";
            accuracyText.text = $"Accuracy: {input.AccuracyPercentage:F0}%";
        }
        if (gameManager) difficultyText.text = $"Difficulty: {gameManager.CurrentDifficulty}";
        if (scoring) scoreText.text = $"Final Fitness Score: {scoring.CurrentScore}";
    }
}
