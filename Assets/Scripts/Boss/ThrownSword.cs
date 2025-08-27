using UnityEngine;

public class ThrownSword : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 7f;        // 날아가는 속도
    public float life = 2f;         // 몇 초 뒤 자동 파괴
    public float damageAmount = 20f; // 플레이어에게 줄 데미지
    private Vector2 direction;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 일정 시간 뒤 자동 삭제
        Destroy(gameObject, life);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            PlayerStats ps = other.GetComponent<PlayerStats>();
            if (ps != null)
            {
                ps.TakeDamage(damageAmount); // 플레이어 체력 감소
            }

            Destroy(gameObject); // 칼 삭제
        }

        // (선택) 벽이나 다른 장애물과 충돌 시에도 삭제
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    public void SetDirection(Vector2 dir)
{
    direction = dir.normalized;

    if (rb == null) rb = GetComponent<Rigidbody2D>();
    rb.linearVelocity = direction * speed;  // ✅ 속도 적용 추가

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);
}

}
