using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public float attackPower = 10f;
    public float moveSpeed = 5f;
    public float maxHP = 100f;
    public float defense = 0f;
    public int grenadeCount = 1;
    public float reloadSpeed = 1f;

    [Header("Leveling")]
    public float currentExp = 0;
    public float expToNextLevel = 100;

    [Header("UI References")]
    public RectTransform hpBar;
    public RectTransform expBar;
    public GameObject gameOverUI;

    private float currentHP;

    // 바의 원래 길이를 저장
    private float hpBarMaxWidth;
    private float expBarMaxWidth;

    void Start()
    {
        currentHP = maxHP;

        if (hpBar != null)
            hpBarMaxWidth = hpBar.sizeDelta.x;  // HP바 원래 길이 저장

        if (expBar != null)
            expBarMaxWidth = expBar.sizeDelta.x; // EXP바 원래 길이 저장

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        UpdateHPUI();
        UpdateExpUI();
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
        currentExp = 0; // ✅ 경험치 초기화
        expToNextLevel *= 1.5f;

        UpdateExpUI();

        Debug.Log($"레벨업! 현재 레벨: {level}");

        // ✅ 레벨업 UI 열기
        FindObjectOfType<LevelUpUI>().OpenUI(this);
    }

    // === 데미지 처리 ===
    public void TakeDamage(float amount)
    {
        float finalDamage = Mathf.Max(amount - defense, 1f);
        currentHP -= finalDamage;
        UpdateHPUI();

        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        if (gameOverUI != null)
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
}
