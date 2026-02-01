using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private FitnessTrackingSystem fitnessSystem;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text scoreText, ammoText, timerText, calText;
    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (scoringSystem == null) scoringSystem = FindFirstObjectByType<ScoringSystem>();
        if (fitnessSystem == null) fitnessSystem = FindFirstObjectByType<FitnessTrackingSystem>();
    }
    private void Update()
    {
        if (player)
        {
            healthSlider.value = (float)player.CurrentHealth / player.MaxHealth;
            ammoText.text = $"Ammo: {player.CurrentAmmo}";
        }
        if (scoringSystem) scoreText.text = $"Score: {scoringSystem.CurrentScore}";
        if (fitnessSystem)
        {
            timerText.text = System.TimeSpan.FromSeconds(fitnessSystem.Duration).ToString(@"mm\:ss");
            calText.text = $"Cal: {fitnessSystem.Calories:F1}";
        }
    }
}