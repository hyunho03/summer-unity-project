using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class EnemyFollow : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public Transform target;
    public float speed = 2f;

    [Header("Separation Settings")]
    public float separationRadius = 0.5f;
    public float separationStrength = 0.5f;
    public LayerMask enemyLayer;

    [Header("Sprite Settings")]
    public Sprite spriteFacingLeft;
    public Sprite spriteFacingRight;

    [Header("Enemy Health Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    [Header("Damage Settings")]
    public float damageInterval = 1f; // 플레이어 공격 주기
    public float damageAmount = 15f;  // 플레이어에게 주는 데미지

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isTouchingPlayer = false;
    private float damageTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // 타겟 자동 할당 (Player 태그)
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // === 플레이어를 향해 이동 ===
        Vector2 toPlayer = ((Vector2)target.position - rb.position).normalized;

        // Separation (적들끼리 겹치지 않도록 밀어내기)
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

        Vector2 moveDir = toPlayer + separation * separationStrength;
        moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        UpdateSpriteDirection();

        // === 플레이어 접촉 시 데미지 주기 ===
        if (isTouchingPlayer)
        {
            damageTimer += Time.fixedDeltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyPlayerDamage();
                damageTimer = 0f;
            }
        }
    }

    void UpdateSpriteDirection()
    {
        if (sr == null || target == null) return;
        sr.sprite = (transform.position.x < target.position.x) ? spriteFacingRight : spriteFacingLeft;
    }

    // === 적이 피해 입음 ===
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} dmg ({currentHealth}/{maxHealth})");

        if (currentHealth <= 0)
        {
            // ✅ PlayerStats에 경험치 지급
            PlayerStats ps = target.GetComponent<PlayerStats>();
            if (ps != null) ps.GainExp(8f); // 죽으면 20 경험치 지급

            Destroy(gameObject);
        }
    }

    // === 플레이어 충돌 감지 ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            damageTimer = damageInterval; // 첫 충돌 즉시 데미지
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    // === 플레이어에게 데미지 적용 ===
    private void ApplyPlayerDamage()
    {
        if (target == null) return;

        PlayerStats ps = target.GetComponent<PlayerStats>();
        if (ps != null)
        {
            ps.TakeDamage(damageAmount);
        }
    }

    // === Scene 뷰에서 Separation 범위 확인용 Gizmo ===
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
