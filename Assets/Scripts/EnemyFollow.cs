using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Player HP & GameOver")]
    public RectTransform expBar; 
    public RectTransform hpBar;  
    public GameObject gameOverUI;

    [Header("Damage Settings")]
    public float damageInterval = 1f; // 데미지 주기 (초)
    public float damageAmount = 20f;  // 🎯 적이 주는 데미지 (HP바 width 단위)

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // 충돌 상태 & 데미지 타이머
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

        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 toPlayer = ((Vector2)target.position - rb.position).normalized;

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

        if (transform.position.x < target.position.x)
            sr.sprite = spriteFacingRight;
        else
            sr.sprite = spriteFacingLeft;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} dmg ({currentHealth}/{maxHealth})");

        if (currentHealth <= 0)
        {
            if (expBar != null)
            {
                Vector2 size = expBar.sizeDelta;
                size.x += 50f;
                expBar.sizeDelta = size;
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            damageTimer = damageInterval; // 첫 부딪힘에 바로 데미지
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

    // 🎯 플레이어 HP 감소 + 게임 오버 체크
    private void ApplyPlayerDamage()
    {
        if (hpBar != null)
        {
            Vector2 size = hpBar.sizeDelta;
            size.x -= damageAmount; // ✅ 고정값 20 대신 damageAmount 사용
            hpBar.sizeDelta = size;

            if (size.x <= 0)
            {
                size.x = 0;
                hpBar.sizeDelta = size;
                if (gameOverUI != null)
                    gameOverUI.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
