using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameStateManager gameManager;
    [SerializeField] private DifficultyScaler difficultyScaler;
    [SerializeField] private PlayerController player;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float spawnWidth = 8f;
    [SerializeField] private float spawnDistance = 30f;
    private List<Enemy> pool = new List<Enemy>();
    private int activeEnemies;
    private float spawnTimer;
    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null) difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (gameManager != null) gameManager.OnStateChanged += HandleStateChange;
    }
    private void OnDestroy() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChange; }
    private void HandleStateChange(GameState newState)
    {
        if (newState == GameState.Running) ResetSpawner();
        else if (newState == GameState.Idle || newState == GameState.Dashboard) ClearEnemies();
    }
    private void ResetSpawner() { activeEnemies = 0; spawnTimer = 0; }
    private void ClearEnemies() { foreach (var e in pool) e.gameObject.SetActive(false); activeEnemies = 0; }
    private void Update()
    {
        if (gameManager == null || !gameManager.IsPlaying) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && activeEnemies < difficultyScaler.MaxEnemies)
        {
            SpawnEnemy();
            spawnTimer = difficultyScaler.EnemySpawnInterval;
        }
    }
    private void SpawnEnemy()
    {
        Enemy enemy = GetFromPool();
        Vector3 pos = player.transform.position + Vector3.forward * spawnDistance + Vector3.right * Random.Range(-spawnWidth, spawnWidth);
        enemy.transform.position = pos;
        enemy.Initialize(player.transform);
        activeEnemies++;
        if (activeEnemies == 1) gameManager.SetStateDirectly(GameState.Combat);
    }
    private Enemy GetFromPool()
    {
        foreach (var e in pool) if (!e.gameObject.activeInHierarchy) return e;
        Enemy newEnemy = Instantiate(enemyPrefab, transform);
        newEnemy.OnEnemyDeath += HandleEnemyDeath;
        pool.Add(newEnemy);
        return newEnemy;
    }
    private void HandleEnemyDeath(Enemy enemy)
    {
        activeEnemies--;
        if (activeEnemies == 0) gameManager.SetStateDirectly(GameState.Running);
    }
}