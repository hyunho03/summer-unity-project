using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Tooltip("�Ѿ��� ��� �ִ� �ð� (��)")]
    public float lifeTime = 2f;
    [Tooltip("�� �Ѿ��� ������ ������")]
    public int damage = 3;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) IDamageable ����ü(Enemy ��)�� ������� ������ ����
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 2) ��(Wall)�� ������� �׳� �ı�
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
