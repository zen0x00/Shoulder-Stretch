using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameStateManager gameManager;

    [SerializeField]
    private DifficultyScaler difficultyScaler;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Transform leftLaneSpawn;

    [SerializeField]
    private Transform rightLaneSpawn;

    [SerializeField]
    private Enemy[] enemyPrefabs;

    [SerializeField]
    private int Zombies = 10;

    [SerializeField]
    private float SpawnDuration = 20f;

    [SerializeField]
    private TextMeshProUGUI wavesText;
    private int spawnedCount;
    private int currentwave = 1;
    private float spawnInterval;

    [SerializeField]
    private float spawnYOffset = 0.1f;

    [SerializeField]
    private AudioManager audioManager;

    public List<Enemy> activeEnemies = new List<Enemy>();
    private float spawnTimer;
    private bool gameStarted;
    private bool waveTransitioning;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameStateManager>();
        if (difficultyScaler == null)
            difficultyScaler = FindFirstObjectByType<DifficultyScaler>();
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();
        if (leftLaneSpawn == null)
            Debug.LogError("[SPAWNER] leftLaneSpawn is not assigned in Inspector.");
        if (rightLaneSpawn == null)
            Debug.LogError("[SPAWNER] rightLaneSpawn is not assigned in Inspector.");
        spawnInterval = SpawnDuration / Zombies;
        wavesText.text = "Wave-1";
        spawnTimer = 2f;
        Debug.Log(
            $"[SPAWNER] Start — gameManager={gameManager}, player={player}, enemyPrefabs count={enemyPrefabs?.Length ?? 0}, spawnInterval={spawnInterval:F1}s"
        );
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsPlaying)
            return;
        if (gameManager.IsPlaying && !gameStarted)
        {
            Debug.Log("[SPAWNER] Game started — beginning wave 1");
            gameStarted = true;
            StartCoroutine(Wavetimer());
        }
        spawnTimer -= Time.deltaTime;
        if (spawnedCount >= Zombies && !AreEnemiesAlive() && !waveTransitioning)
        {
            if (currentwave < 3)
            {
                Debug.Log(
                    $"[SPAWNER] Wave {currentwave} cleared → starting wave {currentwave + 1}"
                );
                waveTransitioning = true;
                currentwave++;
                wavesText.text = "Wave-" + currentwave;
                StartCoroutine(Wavetimer());

                Zombies += 5;
                spawnInterval = SpawnDuration / Zombies;
                spawnedCount = 0;
                spawnTimer = spawnInterval;

                return;
            }
            else
            {
                gameManager.LevelCompleted();
                return;
            }
            ;
        }

        if (spawnTimer <= 0 && spawnedCount < Zombies)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    IEnumerator Wavetimer()
    {
        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
        audioManager?.PlayWavesSound();
        wavesText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        wavesText.gameObject.SetActive(false);
        waveTransitioning = false;
    }

    private void SpawnEnemy()
    {
        Enemy enemy = GetFromPool();
        if (enemy == null)
            return;

        bool spawnLeft = Random.value > 0.5f;
        Transform spawnTransform = spawnLeft ? leftLaneSpawn : rightLaneSpawn;

        if (spawnTransform == null)
        {
            Debug.LogError($"[SPAWNER] {(spawnLeft ? "leftLaneSpawn" : "rightLaneSpawn")} is not assigned in Inspector.");
            return;
        }

        Vector3 rawPos = spawnTransform.position;
        Vector3 pos = rawPos;
        RaycastHit[] hits = Physics.RaycastAll(rawPos + Vector3.up * 10f, Vector3.down, 20f);
        float groundY = float.MaxValue;
        foreach (var h in hits) if (h.point.y < groundY) groundY = h.point.y;
        pos.y = (groundY < float.MaxValue ? groundY : 0f) + spawnYOffset;

        enemy.transform.position = pos;
        enemy.lane = spawnLeft ? Enemy.Lane.Left : Enemy.Lane.Right;
        enemy.Initialize(player.transform);
        spawnedCount++;
        Debug.Log($"[SPAWNER] Spawned enemy {spawnedCount}/{Zombies} in {enemy.lane} lane at {pos}");
    }

    bool AreEnemiesAlive()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    private Enemy GetFromPool()
    {
        foreach (var e in activeEnemies)
            if (!e.gameObject.activeInHierarchy)
                return e;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError(
                "EnemySpawner: enemyPrefabs array is empty — assign prefabs in Inspector."
            );
            return null;
        }
        int index = Random.Range(0, enemyPrefabs.Length);
        Enemy newEnemy = Instantiate(enemyPrefabs[index], transform);
        activeEnemies.Add(newEnemy);
        return newEnemy;
    }
}

