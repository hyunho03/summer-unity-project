using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // 새 Input System 사용 시
#endif

public class TitleController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string nextSceneName = "water"; // 실제 게임 씬 이름

    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private CanvasGroup fadeCanvas; // 전체화면 검은 이미지+CanvasGroup (선택)

    [Header("FX")]
    [SerializeField] private float fadeDuration = 0.35f;

    private bool loading = false;

    private void Awake()
    {
        // 첫 포커스를 Play로
        if (EventSystem.current != null && playButton != null)
        {
            EventSystem.current.firstSelectedGameObject = playButton.gameObject;
            playButton.Select();
        }

        // 페이드 초기화
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f; // 투명
            fadeCanvas.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (loading) return;

        // 키보드 단축키
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                OnClickPlay();
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                OnClickQuit();
        }
        #else
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            OnClickPlay();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClickQuit();
        #endif
    }

    public void OnClickPlay()
    {
        if (loading) return;
        loading = true;

        // 버튼 중복 클릭 방지
        SetButtonsInteractable(false);

        // 페이드+비동기 로딩
        StartCoroutine(LoadGameRoutine());
    }

    public void OnClickQuit()
    {
        if (loading) return;
        loading = true;
        SetButtonsInteractable(false);

        Quit();
    }

    private void SetButtonsInteractable(bool state)
    {
        if (playButton) playButton.interactable = state;
        if (quitButton) quitButton.interactable = state;
    }

    private System.Collections.IEnumerator LoadGameRoutine()
    {
        // 페이드 아웃
        if (fadeCanvas != null)
        {
            fadeCanvas.blocksRaycasts = true;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeCanvas.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = 1f;
        }

        var op = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true; // 바로 활성화 (필요 시 false로 두고 조건 활성화)
        while (!op.isDone) yield return null;
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
