using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Buttons")]
    public Button damageButton;     // 공격력 증가
    public Button grenadeButton;    // 수류탄 개수 증가 (+3)
    public Button hpupButton;
    public Button reroadButton;

    private PlayerStats player;

    void Start()
    {
        panel.SetActive(false);
    }

    public void OpenUI(PlayerStats stats)
    {
        player = stats;
        panel.SetActive(true);
        Time.timeScale = 0f;

        // 공격력 증가 버튼
        damageButton.onClick.RemoveAllListeners();
        damageButton.onClick.AddListener(() =>
        {
            player.IncreaseAttack(5f);
            CloseUI();
        });

        // 수류탄 개수 +3 버튼
        grenadeButton.onClick.RemoveAllListeners();
        grenadeButton.onClick.AddListener(() =>
        {
            player.IncreaseGrenade(2);   // 🎯 수류탄 개수 +2
            player.UpdateGrenadeUI();    // UI 갱신
            CloseUI();
        });
        hpupButton.onClick.RemoveAllListeners();
        hpupButton.onClick.AddListener(() =>
        {
            player.IncreaseHealth(20f);   // 체력증가
            CloseUI();
        });
        reroadButton.onClick.RemoveAllListeners();
        reroadButton.onClick.AddListener(() =>
        {
            player.ReduceReloadTime(0.7f);   // 재장전 속도 10%씩 줄이기
            CloseUI();
        });
    }

    public void CloseUI()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
