using System;
using UnityEngine;
public enum ActionType { None, LeftPunch, RightPunch, PickupGun, Shoot, Shield }
public class InputSystem : MonoBehaviour
{
    public static InputSystem Instance { get; private set; }
    public event Action<ActionType, bool> OnActionPerformed;
    public event Action<ActionType> OnActionAttempted;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;
    [SerializeField] private float punchCooldown = 0.5f;
    [SerializeField] private float pickupCooldown = 1.0f;
    [SerializeField] private float shootCooldown = 0.3f;
    [SerializeField] private float shieldCooldown = 2.0f;
    private float leftPunchTimer, rightPunchTimer, pickupTimer, shootTimer, shieldTimer;
    public int LeftActionCount { get; private set; }
    public int RightActionCount { get; private set; }
    public int SuccessfulActions { get; private set; }
    public int FailedActions { get; private set; }
    public int TotalActions => LeftActionCount + RightActionCount;
    public int TotalAttempts => SuccessfulActions + FailedActions;
    public float AccuracyPercentage => TotalAttempts > 0 ? (float)SuccessfulActions / TotalAttempts * 100f : 100f;
    private bool inputEnabled = false;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null) difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
        ResetStats();
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState newState)
    {
        inputEnabled = newState == GameState.Running || newState == GameState.Combat;
        if (newState == GameState.Running) ResetStats();
    }
    public void ResetStats()
    {
        LeftActionCount = RightActionCount = SuccessfulActions = FailedActions = 0;
        leftPunchTimer = rightPunchTimer = pickupTimer = shootTimer = shieldTimer = 0;
    }
    private void Update()
    {
        if (!inputEnabled) return;
        UpdateCooldowns();
        ProcessInput();
    }
    private void UpdateCooldowns()
    {
        float dt = Time.deltaTime;
        float mult = difficultyScaler?.CooldownMultiplier ?? 1f;
        leftPunchTimer = Mathf.Max(0, leftPunchTimer - dt);
        rightPunchTimer = Mathf.Max(0, rightPunchTimer - dt);
        pickupTimer = Mathf.Max(0, pickupTimer - dt);
        shootTimer = Mathf.Max(0, shootTimer - dt);
        shieldTimer = Mathf.Max(0, shieldTimer - dt);
    }
    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryPerformAction(ActionType.LeftPunch);
        if (Input.GetKeyDown(KeyCode.D)) TryPerformAction(ActionType.RightPunch);
        if (Input.GetKeyDown(KeyCode.W)) TryPerformAction(ActionType.PickupGun);
        if (Input.GetKeyDown(KeyCode.S)) TryPerformAction(ActionType.Shoot);
        if (Input.GetKeyDown(KeyCode.Space)) TryPerformAction(ActionType.Shield);
    }
    private void TryPerformAction(ActionType action)
    {
        OnActionAttempted?.Invoke(action);
        bool success = false;
        float mult = difficultyScaler?.CooldownMultiplier ?? 1f;
        switch (action)
        {
            case ActionType.LeftPunch:
                if (leftPunchTimer <= 0) { leftPunchTimer = punchCooldown * mult; LeftActionCount++; success = true; }
                break;
            case ActionType.RightPunch:
                if (rightPunchTimer <= 0) { rightPunchTimer = punchCooldown * mult; RightActionCount++; success = true; }
                break;
            case ActionType.PickupGun:
                if (pickupTimer <= 0) { pickupTimer = pickupCooldown * mult; success = true; }
                break;
            case ActionType.Shoot:
                if (shootTimer <= 0) { shootTimer = shootCooldown * mult; success = true; }
                break;
            case ActionType.Shield:
                if (shieldTimer <= 0) { shieldTimer = shieldCooldown * mult; success = true; }
                break;
        }
        if (success) SuccessfulActions++;
        else FailedActions++;
        OnActionPerformed?.Invoke(action, success);
    }
    public float GetCooldownProgress(ActionType action)
    {
        float mult = difficultyScaler?.CooldownMultiplier ?? 1f;
        return action switch
        {
            ActionType.LeftPunch => 1f - (leftPunchTimer / (punchCooldown * mult)),
            ActionType.RightPunch => 1f - (rightPunchTimer / (punchCooldown * mult)),
            ActionType.PickupGun => 1f - (pickupTimer / (pickupCooldown * mult)),
            ActionType.Shoot => 1f - (shootTimer / (shootCooldown * mult)),
            ActionType.Shield => 1f - (shieldTimer / (shieldCooldown * mult)),
            _ => 1f
        };
    }
}