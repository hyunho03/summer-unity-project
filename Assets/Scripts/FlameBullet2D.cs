using UnityEngine;

public class FlameBullet2D : MonoBehaviour
{
    public float speed = 7f;
    public float life = 2f;
    public float damageAmount = 15f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, life);
    }

    // 🎯 EnemyFlameShooter에서 호출
    public void Init(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // 불꽃 이동
        rb.linearVelocity = direction.normalized * speed;

        // 🔥 불꽃 스프라이트 회전 (방향에 맞게)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ PlayerStats를 찾아서 데미지 적용
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }

            Destroy(gameObject);
        }
    }
}
