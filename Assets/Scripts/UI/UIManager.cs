using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{              
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private GameObject menuPanel, difficultyPanel, hudPanel, pausePanel, dashboardPanel, analyticsPanel, graphPanel, gameOverPanel, levelCompletedPanel, damagePanel;
    [SerializeField] private Button startButton, beginnerBtn, moderateBtn, expertBtn, dashboardNxtBtn, analyticsNxtBtn, graphNxtBtn, gameOverNextBtn, gameOverRestartBtn, levelCompleteNxtBtn, levelCompleteRestartBtn;
    [SerializeField] private AudioManager audioManager;
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (startButton) startButton.onClick.AddListener(() => { audioManager.PlayButtonClick(); ShowDifficultySelection(); });
        if (beginnerBtn) beginnerBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); StartWithDifficulty(0); });
        if (moderateBtn) moderateBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); StartWithDifficulty(1); });
        if (expertBtn) expertBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); StartWithDifficulty(2); });
        if (dashboardNxtBtn) dashboardNxtBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); ShowAnalytics();});
        if (analyticsNxtBtn) analyticsNxtBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); ShowGraph(); });
        if (graphNxtBtn) graphNxtBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); gameManager.ReturnToIdle(); });
        if (gameOverNextBtn) gameOverNextBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); gameManager.SetStateDirectly(GameState.Dashboard); });
        if (gameOverRestartBtn) gameOverRestartBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); gameManager.ReturnToIdle(); });
        if (levelCompleteNxtBtn) levelCompleteNxtBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); gameManager.SetStateDirectly(GameState.Dashboard); });
        if (levelCompleteRestartBtn) levelCompleteRestartBtn.onClick.AddListener(() => { audioManager.PlayButtonClick(); gameManager.ReturnToIdle(); });

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
            analyticsPanel?.SetActive(false);
            graphPanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
            levelCompletedPanel?.SetActive(false);
        }
        else
        {
            menuPanel?.SetActive(false);
            hudPanel?.SetActive(state == GameState.Running || state == GameState.Combat || state == GameState.Paused);
            pausePanel?.SetActive(state == GameState.Paused);
            dashboardPanel?.SetActive(state == GameState.Dashboard);
            gameOverPanel?.SetActive(state == GameState.GameOver);
            levelCompletedPanel?.SetActive(state == GameState.LevelCompleted);
        }
    }


    private void ShowAnalytics()
    {
        dashboardPanel?.SetActive(false);
        analyticsPanel?.SetActive(true);
    }

    private void ShowGraph()
    {
        analyticsPanel?.SetActive(false);
        graphPanel?.SetActive(true);
    }


    public void ShowDamageBlink()
    {
        StartCoroutine(BlickRoutine());
    }


    public IEnumerator BlickRoutine()
    {
        damagePanel?.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damagePanel?.SetActive(false);
    }


}