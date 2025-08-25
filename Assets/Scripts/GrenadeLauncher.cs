using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [Header("발사 설정")]
    [SerializeField] private GameObject grenadePrefab;   // 수류탄 프리팹
    [SerializeField] private Transform launchPoint;      // 발사 위치(Transform)

    [Header("거리 비례 궤적 설정")]
    [SerializeField] private float desiredHorzSpeed = 5f; // 초당 수평 이동 속도 (거리 비례 시간 계산용)
    [SerializeField] private float minFlightTime = 0.2f; // 최소 비행 시간
    [SerializeField] private float maxFlightTime = 1.0f; // 최대 비행 시간

    private PlayerStats playerStats; // 🎯 플레이어 스탯 참조

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            Debug.LogError("❌ GrenadeLauncher: PlayerStats 컴포넌트를 찾을 수 없음!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        if (playerStats == null) return;

        // 🎯 수류탄 개수 확인
        if (playerStats.grenadeCount <= 0)
        {
            Debug.Log("❌ 수류탄 없음!");
            return;
        }

        // 1) 마우스 화면 좌표 → 월드 좌표 변환
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // 2) 수류탄 인스턴스화
        GameObject grenade = Instantiate(grenadePrefab, launchPoint.position, Quaternion.identity);

        // 3) 목표까지의 변위 및 거리 계산
        Vector2 disp = (Vector2)mouseWorld - (Vector2)launchPoint.position;
        float distance = disp.magnitude;
        float g = Physics2D.gravity.y;

        // 4) 거리 비례 비행 시간(dynamicTime) 계산
        float dynamicTime = distance / desiredHorzSpeed;
        dynamicTime = Mathf.Clamp(dynamicTime, minFlightTime, maxFlightTime);

        // 5) 초기 속도 계산 (vx, vy)
        float vx = disp.x / dynamicTime;
        float vy = (disp.y - 0.5f * g * dynamicTime * dynamicTime) / dynamicTime;
        Vector2 initVel = new Vector2(vx, vy);

        // 6) Rigidbody2D에 속도 할당
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.linearVelocity = initVel;

        // 7) 목표 좌표 전달하여 폭발 시점 결정
        grenade.GetComponent<Grenade>().Initialize(mouseWorld);

        // 🎯 수류탄 사용 → 개수 감소
        playerStats.grenadeCount--;
        playerStats.UpdateGrenadeUI(); // ✅ UI 갱신
        Debug.Log($"💣 수류탄 던짐! 남은 개수: {playerStats.grenadeCount}");
    }
}
