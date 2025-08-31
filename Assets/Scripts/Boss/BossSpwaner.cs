using UnityEngine;
using UnityEngine.UI;   // ✅ Slider
using TMPro;            // ✅ TextMeshPro
using System.Collections;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 5f;

    [Header("UI Settings")]
    public TextMeshProUGUI bossSpawnTimerText;  // 화면 구석 타이머 텍스트
    public GameObject bossHpUI;                 // BossHPBar 전체 오브젝트
    public Slider bossHpSlider;                 // BossHPBar의 Slider 컴포넌트

    private float timer;
    private bool spawned = false;

    void Start()
    {
        timer = spawnDelay;

        // 시작할 땐 HP UI 끄기
        if (bossHpUI != null)
            bossHpUI.SetActive(false);
    }

    void Update()
    {
        if (spawned) return;

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        // 화면 구석에 타이머 표시
        if (bossSpawnTimerText != null)
            bossSpawnTimerText.text = $": {Mathf.CeilToInt(timer)}S";

        // 시간이 다 되면 보스 소환
        if (timer <= 0 && !spawned)
        {
            SpawnBoss();
            spawned = true;

            // 타이머 텍스트는 꺼버리기
            if (bossSpawnTimerText != null)
                bossSpawnTimerText.gameObject.SetActive(false);
        }
    }

    void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        BossWalking bw = boss.GetComponent<BossWalking>();

        if (bw != null)
        {
            bw.bossHpUI = bossHpUI;
            bw.bossHpSlider = bossHpSlider;

            if (bossHpUI != null)
            {
                bossHpUI.SetActive(true); // HP UI 켜기
            }

            if (bossHpSlider != null)
            {
                bossHpSlider.maxValue = bw.MaxHealth;   // 🔹 BossWalking에 public getter 필요
                bossHpSlider.value = bw.MaxHealth;
            }
        }
        // 🔊 보스 등장 시 BGM 교체
        FindFirstObjectByType<BGMManager>()?.PlayBossBGM();

        Debug.Log("보스 소환!");
    }
}
