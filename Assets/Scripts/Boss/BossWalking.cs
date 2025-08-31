using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System; // ✅ 이벤트 사용을 위해 필요

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class BossWalking : MonoBehaviour, IDamageable
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
    [SerializeField] private int maxHealth = 500;
    public int MaxHealth => maxHealth;
    private int currentHealth;

    [Header("Damage Settings")]
    public float damageInterval = 1f; 
    public float damageAmount = 20f;  

    [Header("UI Settings")]
    public GameObject bossHpUI;   // Canvas 안의 BossHPBar
    public Slider bossHpSlider;   // 체력바 Slider

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isTouchingPlayer = false;
    private float damageTimer = 0f;

    // ✅ 보스 사망 이벤트 (외부 UI 매니저에서 구독 가능)
    public static event Action BossDied;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // 타겟 자동 할당
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        // HP UI 연결
        if (bossHpUI == null)
            bossHpUI = GameObject.Find("BossHPBar");

        if (bossHpUI != null)
            bossHpUI.SetActive(true);

        if (bossHpSlider == null && bossHpUI != null)
            bossHpSlider = bossHpUI.GetComponent<Slider>();

        if (bossHpSlider != null)
        {
            bossHpSlider.maxValue = maxHealth;
            bossHpSlider.value = currentHealth;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // === 이동 ===
        Vector2 toPlayer = ((Vector2)target.position - rb.position).normalized;
        sr.flipX = (target.position.x < transform.position.x);

        // Separation
        Vector2 separation = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(rb.position, separationRadius, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;
            Vector2 diff = rb.position - (Vector2)hit.transform.position;
            float dist = diff.magnitude;
            if (dist > 0) separation += diff.normalized / dist;
        }

        Vector2 moveDir = toPlayer + separation * separationStrength;
        moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        UpdateSpriteDirection();

        // === 플레이어 충돌 데미지 ===
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

    // === 데미지 처리 ===
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} dmg ({currentHealth}/{maxHealth})");

        if (bossHpSlider != null) bossHpSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            if (bossHpUI != null) bossHpUI.SetActive(false);

            // ✅ 보스 사망 이벤트 호출
            BossDied?.Invoke();

            Destroy(gameObject);
        }
    }

    // === 플레이어 충돌 감지 ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            damageTimer = damageInterval;
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

    private void ApplyPlayerDamage()
    {
        if (target == null) return;
        PlayerStats ps = target.GetComponent<PlayerStats>();
        if (ps != null)
            ps.TakeDamage(damageAmount);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
