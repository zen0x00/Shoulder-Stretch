using UnityEngine;
public class CombatSystem : MonoBehaviour
{
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private PlayerController player;
    [SerializeField] private float punchRange = 2f;
    [SerializeField] private int punchDamage = 50;
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private int shootDamage = 100;
    private void Start()
    {
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (player == null) player = GetComponent<PlayerController>();
        if (inputSystem != null) inputSystem.OnActionPerformed += HandleAction;
    }
    private void OnDestroy() { if (inputSystem != null) inputSystem.OnActionPerformed -= HandleAction; }
    private void HandleAction(ActionType action, bool success)
    {
        if (!success) return;
        switch (action)
        {
            case ActionType.LeftPunch: PerformPunch(true); break;
            case ActionType.RightPunch: PerformPunch(false); break;
            case ActionType.PickupGun: player?.AddAmmo(3); break;
            case ActionType.Shoot: PerformShoot(); break;
            case ActionType.Shield: player?.ActivateShield(); break;
        }
    }
    private void PerformPunch(bool isLeft)
    {
        Vector3 origin = transform.position + transform.forward + (isLeft ? -transform.right : transform.right);
        Collider[] hits = Physics.OverlapSphere(origin, punchRange);
        foreach (var hit in hits) if (hit.TryGetComponent<Enemy>(out var enemy)) enemy.TakeDamage(punchDamage);
    }
    private void PerformShoot()
    {
        if (player == null || !player.UseAmmo()) return;
        if (Physics.Raycast(transform.position, transform.forward, out var hit, shootRange)) if (hit.collider.TryGetComponent<Enemy>(out var enemy)) enemy.TakeDamage(shootDamage);
    }
}