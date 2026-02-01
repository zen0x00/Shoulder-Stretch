using UnityEngine;
public class SessionEndController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private GameObject hudCanvas, dashboardCanvas;
    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (player != null) player.OnPlayerDeath += HandlePlayerDeath;
    }
    private void OnDestroy() { if (player != null) player.OnPlayerDeath -= HandlePlayerDeath; }
    private void HandlePlayerDeath()
    {
        Time.timeScale = 0f;
        if (hudCanvas) hudCanvas.SetActive(false);
        if (dashboardCanvas) dashboardCanvas.SetActive(true);
        if (gameManager) gameManager.SetStateDirectly(GameState.Dashboard);
    }
}