using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    private Animator anim;

    [Header("Colliders")]
    public PolygonCollider2D walkCollider;  // 걷기용
    public PolygonCollider2D runCollider;   // 달리기용

    [Header("Movement Settings")]
    public float walkSpeed = 2f;   // 걷기 속도
    public float runSpeed = 5f;    // 달리기 속도

    public float thrownSpeed = 0f;  //제자리에서 그냥 걷기
    private BossWalking followScript; // 보스 추적 스크립트 참조, 속도조절을 위해서
    [Header("Sword Settings")]
    public GameObject swordPrefab;    // 칼 프리팹 (Animator 내장)
    public Transform RighthandPoint;   // 칼을 붙일 위치
    public Transform LefthandPoint;

    public GameObject thrownPrefab;   //던지는 칼 프리팹

    void Awake()
    {
        anim = GetComponent<Animator>();
        followScript = GetComponent<BossWalking>();

        // 시작은 걷기 상태
        //걷기,뛰기의 각 히트박스가 달라야 되므로 보스객체에 자식생성하여 뛰기전용 콜라이더 생성 후에 그걸로 변환하기 위해
        //변수 2개 생성
        walkCollider.enabled = true;
        runCollider.enabled = false;
        if (followScript != null) followScript.speed = walkSpeed;
    }

    void Start()
    {
        StartCoroutine(BossPatternLoop());
    }

    IEnumerator BossPatternLoop()
    {
        while (true)
        {
            // 🟢 5초 동안 Walk
            anim.SetBool("isWalking", true);
            walkCollider.enabled = true;
            runCollider.enabled = false;
            if (followScript != null) followScript.speed = walkSpeed;

            yield return new WaitForSeconds(1f);

            // 🔴 공격 시작
            int pattern = Random.Range(1, 4);
            switch (pattern)
            {
                case 1: // 찌르기 → Run 상태
                    Debug.Log("찌르기 패턴 시작");
                    //애니메이터에서 애니메이션 연결선 컨디션값 조절하여 애니메이션클립 변환
                    anim.SetBool("isWalking", false);
                    walkCollider.enabled = false;
                    runCollider.enabled = true;

                    if (followScript != null) followScript.speed = runSpeed; // ✅ 달릴 때 속도 업

                    //5초 현재 상태 유지하는 코드
                    yield return new WaitForSeconds(5f);
                    //다시 걷기모드로 변환후에 종료
                    anim.SetBool("isWalking", true);

                    walkCollider.enabled = true;
                    runCollider.enabled = false;

                    if (followScript != null) followScript.speed = walkSpeed; // ✅ 다시 걷기 속도

                    Debug.Log("찌르기 패턴 종료");
                    break;

                case 2: // 칼 휘두르기
                    Debug.Log("칼 휘두르기 실행");

                    // 칼 프리팹 생성 (보스 손에 붙이기)
                    GameObject sword1 = Instantiate(swordPrefab, RighthandPoint.position, RighthandPoint.rotation, RighthandPoint);
                    GameObject sword2 = Instantiate(swordPrefab, LefthandPoint.position, LefthandPoint.rotation, LefthandPoint);
                    // 칼 Animator 실행
                    Animator swordAnim1 = sword1.GetComponent<Animator>();
                    Animator swordAnim2 = sword2.GetComponent<Animator>();

                    // 휘두르기 애니메이션 길이만큼 대기 (예: 2초)
                    yield return new WaitForSeconds(2f);

                    // 애니메이션 끝나면 칼 오브젝트 제거
                    Destroy(sword1);
                    Destroy(sword2);
                    break;


                case 3: // 칼 던지기
    
                    Debug.Log("칼 던지기 실행");

                    int knifeCount = 8; // 던질 칼 개수 (360도 방향)
                    float angleStep = 360f / knifeCount;
                    float angle = 0f;
                    if (followScript != null) followScript.speed = thrownSpeed;
                    for (int j=0; j<4;j++){
                        angle += 22.5f;
                        for (int i = 0; i < knifeCount; i++)
                        {
                            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
                            Vector2 dir = new Vector2(dirX, dirY);

                            GameObject knife = Instantiate(thrownPrefab, transform.position, Quaternion.identity);
                            
                            ThrownSword ts = knife.GetComponent<ThrownSword>();
                            ts.SetDirection(dir);

                            angle += angleStep;
                        }
                        yield return new WaitForSeconds(1f);
                    }
                
                    if (followScript != null) followScript.speed = walkSpeed;
                    break;

            }

            anim.SetBool("isWalking", true);
            walkCollider.enabled = true;
            runCollider.enabled = false;
            if (followScript != null) followScript.speed = walkSpeed;

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SwordSwing()
    {
        yield return new WaitForSeconds(2f);
    }

    void ThrowKnives(int count)
    {
        // 투사체 발사
    }
}