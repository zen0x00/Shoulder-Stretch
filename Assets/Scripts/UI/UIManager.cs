using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private GameObject menuPanel, difficultyPanel, hudPanel, pausePanel, dashboardPanel;
    [SerializeField] private Button startButton, beginnerBtn, moderateBtn, expertBtn;
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (startButton) startButton.onClick.AddListener(ShowDifficultySelection);
        if (beginnerBtn) beginnerBtn.onClick.AddListener(() => StartWithDifficulty(0));
        if (moderateBtn) moderateBtn.onClick.AddListener(() => StartWithDifficulty(1));
        if (expertBtn) expertBtn.onClick.AddListener(() => StartWithDifficulty(2));
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void ShowDifficultySelection()
    {
        menuPanel?.SetActive(false);
        difficultyPanel?.SetActive(true);
    }
    private void StartWithDifficulty(int index)
    {
        gameManager?.SetDifficulty(index);
        difficultyPanel?.SetActive(false);
        gameManager?.StartGame();
    }
    private void Update()
    {
        if (gameManager == null) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameManager.CurrentState == GameState.Paused) gameManager.ResumeGame();
            else if (gameManager.IsPlaying) gameManager.PauseGame();
        }
    }
    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Idle)
        {
            menuPanel?.SetActive(true);
            difficultyPanel?.SetActive(false);
            hudPanel?.SetActive(false);
            pausePanel?.SetActive(false);
            dashboardPanel?.SetActive(false);
        }
        else
        {
            menuPanel?.SetActive(false);
            hudPanel?.SetActive(state == GameState.Running || state == GameState.Combat || state == GameState.Paused);
            pausePanel?.SetActive(state == GameState.Paused);
            dashboardPanel?.SetActive(state == GameState.Dashboard);
        }
    }
}