using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 새 Input System

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pausePanel;   // Pause UI 루트(비활성으로 두기)
    [SerializeField] GameObject dimmer;       // 선택: 배경 어둡게 패널

    [Header("Options")]
    [SerializeField] bool pauseAudio = true;  // 일시정지 시 오디오 멈춤

    bool paused;

    void Start()
    {
        SetPaused(false, instant: true);
    }

    void Update()
    {
        // ESC 토글 (새 Input System)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
        // 구 Input 백업
        else if (Keyboard.current == null && Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause() => SetPaused(!paused);

    public void SetPaused(bool value, bool instant = false)
{
    paused = value;

    Time.timeScale = paused ? 0f : 1f;

    if (pauseAudio) 
        AudioListener.pause = paused;   // ⬅️ 여기서 true/false 같이 조정됨

    if (pausePanel) pausePanel.SetActive(paused);
    if (dimmer)     dimmer.SetActive(paused);
}


    // ==== 버튼용 ====
    public void OnClickResume()
    {
        SetPaused(false);
    }

    public void OnClickRetry()
    {
        SetPaused(false); // 타임스케일 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClickMainMenu()
{
    SetPaused(false);              // 타임스케일, 오디오 정상화
    SceneManager.LoadScene("Title");
}

}
