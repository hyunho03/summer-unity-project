using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Tooltip("총알이 살아 있는 시간 (초)")]
    public float lifeTime = 2f;
    [Tooltip("이 총알이 입히는 데미지")]
    public int damage = 3;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) IDamageable 구현체(Enemy 등)에 닿았으면 데미지 전달
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 2) 벽(Wall)에 닿았으면 그냥 파괴
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
