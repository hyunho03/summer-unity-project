using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 1f;
    public float spawnDistance = 5f;
    public int maxEnemiesInField = 20; // 동시에 존재할 수 있는 최대 적 수

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null) return;

        // 현재 씬에 있는 적 수 확인
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemies >= maxEnemiesInField) return;

        // 사방(대각 포함) 중 하나 방향 선택
        Vector2[] directions = {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            (Vector2.up + Vector2.left).normalized,
            (Vector2.up + Vector2.right).normalized,
            (Vector2.down + Vector2.left).normalized,
            (Vector2.down + Vector2.right).normalized
        };

        Vector2 direction = directions[Random.Range(0, directions.Length)];
        Vector2 spawnPos = (Vector2)player.position + direction * spawnDistance;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.GetComponent<EnemyFollow>().target = player;
    }
}
