using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState { Idle, Running, Combat, Dashboard, Paused, GameOver, LevelCompleted }
public enum Difficulty { Beginner, Moderate, Expert }
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public event Action<GameState> OnStateChanged;
    public event Action<Difficulty> OnDifficultyChanged;
    [SerializeField] private GameState currentState = GameState.Idle;
    [SerializeField] private Difficulty currentDifficulty = Difficulty.Beginner;
    [SerializeField] private Enemy enemy;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemySpawner enemySpawner;
    
    [SerializeField] private GameObject EnemySpawnerObj;

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
    private void Start()
    {
        Debug.Log("[GSM] Start — setting Idle state");
        currentState = GameState.GameOver; // force non-Idle so SetState fires the event on scene load
        SetState(GameState.Idle);
    }
    public void SetDifficulty(Difficulty difficulty)
    {
        Debug.Log($"[GSM] Difficulty set → {difficulty}");
        currentDifficulty = difficulty;
        OnDifficultyChanged?.Invoke(difficulty);
    }
    public void SetDifficulty(int index) => SetDifficulty((Difficulty)Mathf.Clamp(index, 0, 2));
    public void StartGame()
    {
        Debug.Log("[GSM] StartGame called");
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
    public void LevelCompleted()
    {
        Debug.Log("[GSM] LevelCompleted");
        Time.timeScale = 1f;
        SetState(GameState.LevelCompleted);
        if (EnemySpawnerObj != null) EnemySpawnerObj.SetActive(false);
        if (playerController != null) playerController.gameObject.SetActive(false);
        if (enemySpawner != null) enemySpawner.gameObject.SetActive(false);
    }

    public void EndGame()
    {
        Debug.Log("[GSM] EndGame — player died");
        Time.timeScale = 1f;
        SetState(GameState.GameOver);
        if (enemy != null) enemy.enabled = false;
        if (EnemySpawnerObj != null) EnemySpawnerObj.SetActive(false);
        if (playerController != null) playerController.gameObject.SetActive(false);
        if (enemySpawner != null) enemySpawner.gameObject.SetActive(false);
    }
    public void ReturnToIdle()
    {
        Time.timeScale = 1f;
        SetState(GameState.Idle);
        EnemySpawnerObj.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
            Debug.Log($"[GSM] State: {currentState} → {newState}");
            currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
        else
        {
            Debug.Log($"[GSM] SetState ignored — already in {newState}");
        }
    }
}