using UnityEngine;
public class SafetySystem : MonoBehaviour
{
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private float maxActionRate = 2.0f;
    private float lastActionTime;
    private float actionDelta;
    private void Start()
    {
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (inputSystem != null) inputSystem.OnActionPerformed += (a, s) => CheckSafety();
    }
    private void CheckSafety()
    {
        float now = Time.time;
        actionDelta = now - lastActionTime;
        lastActionTime = now;
        if (actionDelta < (1f / maxActionRate)) Debug.LogWarning("Fatigue Warning: Slow down!");
    }
}