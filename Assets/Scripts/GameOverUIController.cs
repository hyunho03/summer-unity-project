using UnityEngine;
using UnityEngine.SceneManagement; // 🎯 씬 로드용 네임스페이스

public class GameOverUIController : MonoBehaviour
{
    public void RetryGame()
    {
        // 게임 시간 정상화 (죽었을 때 0으로 멈췄으니까)
        Time.timeScale = 1f;

        // 현재 활성화된 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
