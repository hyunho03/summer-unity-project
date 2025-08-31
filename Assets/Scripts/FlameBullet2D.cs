using UnityEngine;

public class FlameBullet2D : MonoBehaviour
{
    public float speed = 7f;
    public float life = 2f;
    public float damageAmount = 15f;

    private Rigidbody2D rb;
    private RectTransform hpBar;
    private GameObject gameOverUI;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // ✅ HP바 자동 연결
        var hpObj = GameObject.Find("HP_bar");
        if (hpObj != null)
            hpBar = hpObj.GetComponent<RectTransform>();

        // ✅ GameOver UI 자동 연결
        var goUI = GameObject.Find("GameOverUI");
        if (goUI != null)
            gameOverUI = goUI;
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
            ApplyPlayerDamage();
            Destroy(gameObject);
        }
    }

    private void ApplyPlayerDamage()
    {
        if (hpBar != null)
        {
            Vector2 size = hpBar.sizeDelta;
            size.x -= damageAmount;
            hpBar.sizeDelta = size;

            if (size.x <= 0)
            {
                size.x = 0;
                hpBar.sizeDelta = size;

                if (gameOverUI != null)
                    gameOverUI.SetActive(true);

                Time.timeScale = 0f; // 게임 정지
            }
        }
    }
}
