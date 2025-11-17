using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int startEnemies = 3;
    public int maxEnemies = 15;
    public float spawnDelay = 0.3f;

    private int currentWaveCount;
    private int aliveEnemies;
    private bool spawning = false;

    void Start()
    {
        StartNewWave(startEnemies);
    }

    void StartNewWave(int amount)
    {
        if (spawning) return;

        spawning = true;
        currentWaveCount = amount;
        aliveEnemies = amount;

        StartCoroutine(SpawnWaveRoutine());
    }

    System.Collections.IEnumerator SpawnWaveRoutine()
    {
        for (int i = 0; i < currentWaveCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }

        spawning = false;
    }

    void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);

        // Suscribirse al evento de muerte
        Health hp = enemy.GetComponent<Health>();
        hp.OnDeath += OnEnemyDeath;
    }

    void OnEnemyDeath()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            // ¿Hay más oleadas?
            if (currentWaveCount < maxEnemies)
            {
                int nextAmount = Mathf.Min(currentWaveCount + 2, maxEnemies);
                StartNewWave(nextAmount);
            }
            else
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        Debug.Log("Todos los enemigos eliminados.");
    }
}
