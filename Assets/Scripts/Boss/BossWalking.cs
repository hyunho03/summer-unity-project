using System.Collections;
using UnityEngine;
using UnityEngine.UI; // ✅ UI 제어 위해 필요

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
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;
    private int currentHealth;

    [Header("Damage Settings")]
    public float damageInterval = 1f; 
    public float damageAmount = 20f;  

    [Header("UI Settings")]
    public GameObject bossHpUI;   // Canvas 안의 BossHPBar (전체 오브젝트)
    public Slider bossHpSlider;   // Slider 컴포넌트 (실제 체력 게이지)

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

    // 타겟 자동 할당
    if (target == null)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    // ✅ UI 자동 연결 (씬에 있는 BossHPBar 찾아오기)
    if (bossHpUI == null)
        bossHpUI = GameObject.Find("BossHPBar"); // Hierarchy 이름 기준

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

        // === 플레이어를 향해 이동 ===
        Vector2 toPlayer = ((Vector2)target.position - rb.position).normalized;

        if (target.position.x < transform.position.x)
            sr.flipX = true;
        else
            sr.flipX = false;

        // Separation (적들끼리 겹치지 않도록)
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

        // HP UI 반영
        if (bossHpSlider != null) bossHpSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            if (bossHpUI != null) bossHpUI.SetActive(false); // UI 끄기
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
        {
            ps.TakeDamage(damageAmount);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
