using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class EnemyFollow : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public Transform target;
    public float speed = 2f;

    [Header("Separation Settings")]
    [Tooltip("적들끼리 어느 반경 안에서 피해야 할지")]
    public float separationRadius = 0.5f;
    [Tooltip("분리 힘의 세기")]
    public float separationStrength = 0.5f;
    [Tooltip("Enemy 레이어만 필터링")]
    public LayerMask enemyLayer;

    [Header("Sprite Settings")]
    [Tooltip("왼쪽 바라보는 스프라이트")]
    public Sprite spriteFacingLeft;
    [Tooltip("오른쪽 바라보는 스프라이트")]
    public Sprite spriteFacingRight;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();  // SpriteRenderer 가져오기
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 1) 플레이어 쪽으로 가는 방향
        Vector2 toPlayer = ((Vector2)target.position - rb.position).normalized;

        // 2) 주변 적들과의 분리 벡터 계산
        Vector2 separation = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(rb.position, separationRadius, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;
            Vector2 diff = rb.position - (Vector2)hit.transform.position;
            float dist = diff.magnitude;
            if (dist > 0)
                separation += diff.normalized / dist;  
        }

        // 3) 합치고 정규화
        Vector2 moveDir = toPlayer + separation * separationStrength;
        moveDir.Normalize();

        // 4) 이동
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        // 5) 스프라이트 방향 업데이트
        UpdateSpriteDirection();
    }

    void UpdateSpriteDirection()
    {
        if (sr == null || target == null) return;

        // 적이 플레이어보다 왼쪽에 있으면 오른쪽 바라보게
        if (transform.position.x < target.position.x)
            sr.sprite = spriteFacingRight;
        else
            sr.sprite = spriteFacingLeft;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} dmg ({currentHealth}/{maxHealth})");
        if (currentHealth <= 0) Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
