using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private GameObject menuPanel, hudPanel, pausePanel, dashboardPanel;
    [SerializeField] private Button startButton, beginnerBtn, moderateBtn, expertBtn;
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (startButton) startButton.onClick.AddListener(() => gameManager?.StartGame());
        if (beginnerBtn) beginnerBtn.onClick.AddListener(() => gameManager?.SetDifficulty(0));
        if (moderateBtn) moderateBtn.onClick.AddListener(() => gameManager?.SetDifficulty(1));
        if (expertBtn) expertBtn.onClick.AddListener(() => gameManager?.SetDifficulty(2));
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
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
        menuPanel?.SetActive(state == GameState.Idle);
        hudPanel?.SetActive(state == GameState.Running || state == GameState.Combat || state == GameState.Paused);
        pausePanel?.SetActive(state == GameState.Paused);
        dashboardPanel?.SetActive(state == GameState.Dashboard);
    }
}