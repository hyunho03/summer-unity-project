using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject victoryPanel;

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false); // 시작 시 꺼둠
    }

    void OnEnable()
    {
        BossWalking.BossDied += ShowVictory;
    }

    void OnDisable()
    {
        BossWalking.BossDied -= ShowVictory;
    }

    void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // 선택: 게임 정지
        }
    }
}
