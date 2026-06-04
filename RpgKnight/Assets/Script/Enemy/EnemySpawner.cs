using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("敌人预制体列表")]
    public GameObject[] enemyPrefabs;

    [Header("生成设置")]
    public float spawnInterval = 5f;
    public int initialPoolSize = 5;
    public int maxPoolSize = 10;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;

        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab != null)
            {
                PoolManager.Instance.CreatePool(prefab, initialPoolSize, maxPoolSize);
            }
        }
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        if (enemyPrefab == null) return;

        GameObject enemy = PoolManager.Instance.GetObject(enemyPrefab);
        if (enemy != null)
        {
            enemy.transform.position = transform.position;
            enemy.transform.rotation = Quaternion.identity;

            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.OnSpawn();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1f);
    }
}
