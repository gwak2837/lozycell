using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    private float spawnInterval;
    private float spawnRadius;

    private Transform playerTransform;
    private float timer;
    private bool isSpawning = true;

    private void Start()
    {
        spawnInterval = GameConfig.Spawner.EnemySpawnInterval;
        spawnRadius = GameConfig.Spawner.EnemySpawnRadius;

        // Find player
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!isSpawning || playerTransform == null)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
            return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerTransform.position + (Vector3)(randomDir * spawnRadius);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.Initialize(playerTransform);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}
