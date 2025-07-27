using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    void Awake()
    {
        // 이미 씬에 UIManager 인스턴스가 있으면 중복 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 이 인스턴스를 싱글턴으로 등록
        instance = this;

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }
}