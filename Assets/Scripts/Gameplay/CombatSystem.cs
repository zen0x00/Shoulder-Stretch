using UnityEngine;
public class CombatSystem : MonoBehaviour
{
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private PlayerController player;
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private int shootDamage = 100;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private BulletTracer bulletTracer;
    [SerializeField] private ParticleSystem muzzelFlash;
    [SerializeField] private Transform barrel;

    [SerializeField] private Transform ShootLeftObj;

    [SerializeField] private Transform ShootRightObj;



    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (player == null) player = GetComponent<PlayerController>();
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (inputSystem != null) inputSystem.OnActionPerformed += HandleAction;
    }
    private void OnDestroy() { if (inputSystem != null) inputSystem.OnActionPerformed -= HandleAction; }
    private void HandleAction(ActionType action, bool success)
    {
        if (!success) return;
        switch (action)
        {
            case ActionType.leftShoot: PerforLeftShoot(); break;
            case ActionType.rightShoot: PerforRightShoot(); break;
        }
    }

    
    private void PerforLeftShoot()
    {
        if (player == null || !player.UseAmmo()) return;

        ShootAtLane(Enemy.Lane.Left);
        audioManager.PlayGunShot();
        cameraFollow.Shake(0.1f, 0.2f);
    }

    private void PerforRightShoot()
    {
        if (player == null || !player.UseAmmo()) return;

        ShootAtLane(Enemy.Lane.Right);
        audioManager.PlayGunShot();
        cameraFollow.Shake(0.1f, 0.2f);
    }


    private void ShootAtLane(Enemy.Lane targetLane)
    {
        Enemy closest = null;
        float minDist = shootRange;

        foreach(Enemy enemy in spawner.activeEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;
            if(enemy.lane != targetLane) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if(dist < minDist)
            {
                closest = enemy;
                minDist = dist;
            }
        }

        Transform empty = targetLane == Enemy.Lane.Left ? ShootLeftObj : ShootRightObj;

        Vector3 endPoint = closest != null? closest.transform.position : empty.position;
        endPoint = new Vector3(endPoint.x, endPoint.y + 3f, endPoint.z);

        muzzelFlash.Play();
        bulletTracer.Fire(barrel.position, endPoint);

        
        if (closest != null) 
        {

            closest.TakeDamage(shootDamage);

        }

    }
}