using System;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnAmmoChanged;
    public event Action<bool> OnShieldStatusChanged;
    public event Action OnPlayerDeath;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private int maxAmmo = 10;
    private int currentAmmo;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentAmmo => currentAmmo;

    [SerializeField] private UIManager uiManager;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null) difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        Debug.Log($"[PLAYER] Start — gameManager={gameManager}, difficultyScaler={difficultyScaler}, uiManager={uiManager}, audioManager={audioManager}");
        ResetPlayer();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState newState) { if (newState == GameState.Running) ResetPlayer(); }
    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        currentAmmo = difficultyScaler?.InitialAmmo ?? 5;
        transform.position = new Vector3(0, 0, 0);
        Debug.Log($"[PLAYER] Reset — health={currentHealth}, ammo={currentAmmo}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnAmmoChanged?.Invoke(currentAmmo);
        OnShieldStatusChanged?.Invoke(false);
    }
    private void Update()
    {
        if (gameManager == null || !gameManager.IsPlaying) return;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[PLAYER] TakeDamage {damage} — health now {currentHealth}");
        audioManager.PlayPlayerDamageTakenSound();
        uiManager?.ShowDamageBlink();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        Debug.Log("[PLAYER] Died");
        OnPlayerDeath?.Invoke();
        gameManager?.EndGame();
    }
    public void AddAmmo(int amount) { currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo); OnAmmoChanged?.Invoke(currentAmmo); }
    public bool UseAmmo()
    {
        if (currentAmmo > 0) { currentAmmo--; OnAmmoChanged?.Invoke(currentAmmo); return true; }
        Debug.Log("[PLAYER] UseAmmo — out of ammo");
        return false;
    }
    public void ActivateShield()
    {
        OnShieldStatusChanged?.Invoke(true);
    }
    public void AmmoReload(int amount)
    {
        currentAmmo = Mathf.Min(CurrentAmmo+ amount, maxAmmo);
        OnAmmoChanged?.Invoke(currentAmmo);
    }
}