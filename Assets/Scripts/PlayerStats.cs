using UnityEngine;
using TMPro;
using System.Collections; // ✅ 코루틴 사용

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public float attackPower = 10f;
    public float moveSpeed = 5f;
    public float maxHP = 100f;
    public float defense = 0f;
    public int grenadeCount = 3;
    public float reloadSpeed = 1f;

    [Header("Leveling")]
    public float currentExp = 0;
    public float expToNextLevel = 100;

    [Header("UI References")]
    public RectTransform hpBar;
    public RectTransform expBar;
    public GameObject gameOverUI;
    public TextMeshProUGUI grenadeText;

    [Header("Audio")] // 🎵 추가
    public AudioSource audioSource;   // 데미지 효과음 재생용
    public AudioClip damageClip;      // 데미지 효과음 클립

    private float currentHP;

    // 바의 원래 길이를 저장
    private float hpBarMaxWidth;
    private float expBarMaxWidth;

    // ✅ 무적 관련
    private bool isInvincible = false;
    public float invincibleDuration = 1.5f;   // 무적 시간
    public float blinkInterval = 0.2f;        // 깜빡임 간격
    private SpriteRenderer spriteRenderer;    // 깜빡임 표시용

    void Start()
    {
        currentHP = maxHP;

        if (hpBar != null)
            hpBarMaxWidth = hpBar.sizeDelta.x;  

        if (expBar != null)
            expBarMaxWidth = expBar.sizeDelta.x; 

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        UpdateHPUI();
        UpdateExpUI();
        UpdateGrenadeUI();

        // ✅ 캐릭터 스프라이트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 🎵 AudioSource 자동 연결 (없으면 찾아서 할당)
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void UpdateGrenadeUI()
    {
        if (grenadeText != null)
            grenadeText.text = $"X {grenadeCount}";
    }

    // === 경험치 획득 ===
    public void GainExp(float amount)
    {
        currentExp += amount;
        UpdateExpUI();

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp = 0; 
        expToNextLevel *= 1.5f;

        UpdateExpUI();

        Debug.Log($"레벨업! 현재 레벨: {level}");

        FindFirstObjectByType<LevelUpUI>()?.OpenUI(this);
    }

    // === 데미지 처리 ===
    public void TakeDamage(float amount)
    {
        if (isInvincible) return; // ✅ 무적 상태면 데미지 무시

        float finalDamage = Mathf.Max(amount - defense, 1f);
        currentHP -= finalDamage;
        UpdateHPUI();

        // 🎵 데미지 효과음 재생
        if (audioSource != null && damageClip != null)
            audioSource.PlayOneShot(damageClip);

        if (currentHP <= 0)
            Die();
        else
            StartCoroutine(InvincibilityRoutine()); // ✅ 무적 코루틴 시작
    }

    // ✅ 무적 + 깜빡임 코루틴
    // PlayerStats.cs 안 InvincibilityRoutine() 수정
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        Collider2D playerCol = GetComponent<Collider2D>();

        // 🔹 무적 시작: 적과 충돌 무시
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            if (enemyCol != null && playerCol != null)
                Physics2D.IgnoreCollision(playerCol, enemyCol, true);
        }

        // 🔁 깜빡임
        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // 🔹 무적 끝: 다시 충돌 허용
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            if (enemyCol != null && playerCol != null)
                Physics2D.IgnoreCollision(playerCol, enemyCol, false);
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }



    private void Die()
    {
        Debug.Log("플레이어 사망!");
        if (gameOverUI != null)
            FindFirstObjectByType<BGMManager>()?.PlayGameOverBGM();
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }

    // === UI 갱신 함수 ===
    private void UpdateHPUI()
    {
        if (hpBar != null)
        {
            float ratio = currentHP / maxHP;
            hpBar.sizeDelta = new Vector2(ratio * hpBarMaxWidth, hpBar.sizeDelta.y);
        }
    }

    private void UpdateExpUI()
    {
        if (expBar != null)
        {
            float ratio = currentExp / expToNextLevel;
            expBar.sizeDelta = new Vector2(ratio * expBarMaxWidth, expBar.sizeDelta.y);
        }
    }
    
    // === 레벨업 보상용 함수들 ===
    public void IncreaseAttack(float value) => attackPower += value;
    public void ReduceIncomingDamage(float value) => defense += value;
    public void ReduceReloadTime(float percent) => reloadSpeed *= (1f - percent);
    public void IncreaseGrenade(int amount) => grenadeCount += amount;

    public void IncreaseHealth(float value)
    {
        maxHP += value;

        // 현재 체력도 같이 늘려서 비율 유지
        currentHP += value;

        if (currentHP > maxHP)
            currentHP = maxHP;

        UpdateHPUI();
    }
}
