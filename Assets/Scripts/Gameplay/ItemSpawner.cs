using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;

    private float spawnRadius;
    private float spawnInterval;

    // Events
    public event Action<NucleobaseType> OnBaseCollected;

    private float timer;
    private Transform playerTransform;
    private Queue<Nucleobase> pool = new Queue<Nucleobase>();
    private bool isSpawning = true;

    private void Start()
    {
        spawnRadius = GameConfig.Spawner.ItemSpawnRadius;
        spawnInterval = GameConfig.Spawner.ItemSpawnInterval;

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!isSpawning)
            return;

        // Update player ref if missing (e.g. player respawned)
        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                playerTransform = player.transform;
            else
                return; // Player not found, stop spawning logic
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    private void SpawnItem()
    {
        if (itemPrefab == null || playerTransform == null)
            return;

        Vector3 center = playerTransform.position;
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = center + new Vector3(randomPos.x, randomPos.y, 0);

        Nucleobase item = GetFromPool();
        if (item != null)
        {
            item.transform.position = spawnPos;
            item.transform.rotation = Quaternion.identity;

            // Initialize
            NucleobaseType randomType = (NucleobaseType)Random.Range(0, 4);
            item.Initialize(randomType);
        }
    }

    private Nucleobase GetFromPool()
    {
        Nucleobase item = null;
        if (pool.Count > 0)
        {
            item = pool.Dequeue();
            if (item != null)
            {
                item.gameObject.SetActive(true);
            }
        }

        if (item == null)
        {
            GameObject obj = Instantiate(itemPrefab);
            item = obj.GetComponent<Nucleobase>();
            if (item == null)
            {
                Debug.LogError("Nucleobase component is missing on the ItemPrefab! Please check the prefab.");
                // Try to add it if missing
                item = obj.AddComponent<Nucleobase>();
            }
        }

        ConfigureItemEvents(item);
        return item;
    }

    // Dictionary to track handlers for clean unsubscription
    private Dictionary<Nucleobase, Action<NucleobaseType>> collectedHandlers =
        new Dictionary<Nucleobase, Action<NucleobaseType>>();

    private void ConfigureItemEvents(Nucleobase item)
    {
        if (item == null)
            return;

        // Remove existing handlers if any (for safety)
        if (collectedHandlers.ContainsKey(item))
        {
            ReturnToPool(item); // Should have been returned, but clean up if here
        }

        Action<NucleobaseType> handler = (type) =>
        {
            OnBaseCollected?.Invoke(type);
            // When collected, we also want to return to pool.
            // We can just call ReturnToPool directly here.
            ReturnToPool(item);
        };

        collectedHandlers[item] = handler;
        item.OnCollected += handler;
        item.OnDespawn += ReturnToPool;
    }

    private void ReturnToPool(Nucleobase item)
    {
        if (item == null)
            return;

        // Unsubscribe OnCollected
        if (collectedHandlers.TryGetValue(item, out var handler))
        {
            item.OnCollected -= handler;
            collectedHandlers.Remove(item);
        }

        // Unsubscribe OnDespawn
        item.OnDespawn -= ReturnToPool;

        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    public void SpawnDrop(Vector3 position)
    {
        if (itemPrefab == null)
            return;

        Nucleobase item = GetFromPool();
        if (item != null)
        {
            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;

            NucleobaseType randomType = (NucleobaseType)Random.Range(0, 4);
            item.Initialize(randomType);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}
