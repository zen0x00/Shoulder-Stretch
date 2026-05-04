using System;
using UnityEngine;
public class SessionEndController : MonoBehaviour
{
    [SerializeField] private FitnessTrackingSystem fitness;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private InputSystem input;
    [SerializeField] private ScoringSystem score;

    public static GameData currentSession {  get; private set; }

    private void Start()
    {
        if (fitness == null) fitness = FindFirstObjectByType<FitnessTrackingSystem>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (input == null) input = FindFirstObjectByType<InputSystem>();
        if (score == null) score = FindFirstObjectByType<ScoringSystem>();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {
        if(state == GameState.GameOver || state == GameState.LevelCompleted){
            currentSession = new GameData
            {
                time = $"{System.TimeSpan.FromSeconds(fitness.Duration):mm\\:ss}",
                calories = Mathf.Round(fitness.Calories),
                leftActions = input.LeftActionCount,
                rightActions = input.RightActionCount,
                accuracy = (float) Math.Round(input.AccuracyPercentage, 2),
                finalScore = score.CurrentScore
            };

        }
    }



}