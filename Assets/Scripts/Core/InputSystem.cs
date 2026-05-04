using System;
using UnityEngine;
public enum ActionType { None, leftShoot, rightShoot}
public class InputSystem : MonoBehaviour
{
    public static InputSystem Instance { get; private set; }
    public event Action<ActionType, bool> OnActionPerformed;
    public event Action<ActionType> OnActionAttempted;
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;


    public int LeftActionCount { get; private set; }
    public int RightActionCount { get; private set; }
    public int SuccessfulActions { get; private set; }
    public int FailedActions { get; private set; }
    public int TotalActions => LeftActionCount + RightActionCount;
    public int TotalAttempts => SuccessfulActions + FailedActions;
    public float AccuracyPercentage => TotalAttempts > 0 ? (float)SuccessfulActions / TotalAttempts * 100f : 100f;
    private bool inputEnabled = false;

    [SerializeField] Animator animator;
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
        
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState newState)
    {
        inputEnabled = newState == GameState.Running || newState == GameState.Combat;
        Debug.Log($"[INPUT] State → {newState}, inputEnabled={inputEnabled}");
    }

    private void Update()
    {
        if (!inputEnabled) return;
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("[INPUT] A pressed → leftShoot");
            TryPerformAction(ActionType.leftShoot);
            if (animator != null) animator.SetTrigger("LeftShoot");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("[INPUT] D pressed → rightShoot");
            TryPerformAction(ActionType.rightShoot);
            if (animator != null) animator.SetTrigger("RightShoot");
        }

    }
    private void TryPerformAction(ActionType action)
    {
        OnActionAttempted?.Invoke(action);
        bool success = false;
        switch (action)
        {
            case ActionType.leftShoot:
                LeftActionCount++;
                success = true;
                break;

            case ActionType.rightShoot:
                RightActionCount++;
                success = true;
                break;
        }
        if (success) SuccessfulActions++;
        else FailedActions++;
        OnActionPerformed?.Invoke(action, success);
    }

}