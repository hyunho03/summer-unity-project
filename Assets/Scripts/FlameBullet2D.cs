using UnityEngine;

public class FlameBullet2D : MonoBehaviour
{
    public float speed = 7f;
    public float life = 2f;
    public float damageAmount = 20f;

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

    // 🎯 EnemyFlameShooter나 MouthShooter에서 호출하는 초기화 함수
    public void Init(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;
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
