using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    public GameObject panel;
    public Button attackButton;   // 데미지 증가 버튼
    private PlayerStats player;

    void Start()
    {
        panel.SetActive(false);
    }

    public void OpenUI(PlayerStats stats)
    {
        player = stats;
        panel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지

        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(() =>
        {
            player.IncreaseAttack(5f); // 공격력 +5
            CloseUI();
        });
    }

    public void CloseUI()
    {
        panel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }
}
