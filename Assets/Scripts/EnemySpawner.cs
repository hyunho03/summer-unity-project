using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 1f;     // 1초 간격
    public float spawnDistance = 5f;     // 플레이어로부터 떨어진 거리

    void Start()
    {
        // 첫 생성 1초 지연 → 그 뒤로 매 1초마다 반복
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }


    void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null) return;

        // 🔁 사방 중 하나 방향 선택
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
