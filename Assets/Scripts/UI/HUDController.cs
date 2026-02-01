using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText, timerText, difficultyText, ammoText;
    private Image shieldIcon;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private ScoringSystem scoring;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private FitnessTrackingSystem fitness;
    private void Start()
    {
        if (scoring == null) scoring = FindFirstObjectByType<ScoringSystem>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (fitness == null) fitness = FindFirstObjectByType<FitnessTrackingSystem>();
        if (scoring) scoring.OnScoreChanged += s => scoreText.text = $"Score: {s}";
        if (player)
        {
            player.OnAmmoChanged += a => ammoText.text = $"Ammo: {a}";
            player.OnShieldStatusChanged += active => shieldIcon.enabled = active;
            player.OnHealthChanged += (h, m) => healthSlider.value = (float)h / m;
        }
        if (gameManager)
        {
            gameManager.OnDifficultyChanged += d => difficultyText.text = $"Difficulty: {d}";
            difficultyText.text = $"Difficulty: {gameManager.CurrentDifficulty}";
        }
        shieldIcon.enabled = false;
    }
    private void Update()
    {
        if (fitness)
        {
            TimeSpan t = TimeSpan.FromSeconds(fitness.Duration);
            timerText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }
}
