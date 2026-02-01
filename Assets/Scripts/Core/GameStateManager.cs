using System;
using UnityEngine;
public enum GameState { Idle, Running, Combat, Dashboard, Paused }
public enum Difficulty { Beginner, Moderate, Expert }
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public event Action<GameState> OnStateChanged;
    public event Action<Difficulty> OnDifficultyChanged;
    [SerializeField] private GameState currentState = GameState.Idle;
    [SerializeField] private Difficulty currentDifficulty = Difficulty.Beginner;
    public GameState CurrentState => currentState;
    public Difficulty CurrentDifficulty => currentDifficulty;
    public bool IsPlaying => currentState == GameState.Running || currentState == GameState.Combat;
    public bool IsPaused => currentState == GameState.Paused;
    private float sessionStartTime;
    public float SessionDuration => Time.time - sessionStartTime;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void Start() => SetState(GameState.Idle);
    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        OnDifficultyChanged?.Invoke(difficulty);
    }
    public void SetDifficulty(int index) => SetDifficulty((Difficulty)Mathf.Clamp(index, 0, 2));
    public void StartGame()
    {
        sessionStartTime = Time.time;
        SetState(GameState.Running);
    }
    public void PauseGame()
    {
        if (IsPlaying)
        {
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }
    }
    public void ResumeGame()
    {
        if (IsPaused)
        {
            Time.timeScale = 1f;
            SetState(GameState.Running);
        }
    }
    public void EndGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Dashboard);
    }
    public void ReturnToIdle()
    {
        Time.timeScale = 1f;
        SetState(GameState.Idle);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        sessionStartTime = Time.time;
        SetState(GameState.Running);
    }
    public void SetStateDirectly(GameState newState) => SetState(newState);
    private void SetState(GameState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}