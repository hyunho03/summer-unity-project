using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    private Vector2 targetPos;
    private bool initialized = false;

    [Header("폭발 설정")]
    [SerializeField] private float arriveThreshold = 0.1f;   // 목표 도달 판정 거리
    [SerializeField] private float explodeRadius = 1.5f;   // 데미지 범위
    [SerializeField] private GameObject explosionPrefab;        // 폭발 이펙트 프리팹
    [SerializeField] private float explodeDelay = 0f;     // 폭발 지연(초)

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // 스폰 직후 콜라이더가 겹치며 바로 터지는 문제 방지
        col.enabled = false;
        StartCoroutine(EnableColliderWithDelay());
    }

    /// <summary>
    /// 콜라이더를 일정 시간(여기서는 0.2초) 기다린 뒤 활성화합니다.
    /// </summary>
    private IEnumerator EnableColliderWithDelay()
    {
        // 원하는 시간만큼 대기 (예: 0.2초)
        yield return new WaitForSeconds(0.2f);
        col.enabled = true;
    }

    /// <summary>
    /// 발사기에서 호출: 목표 좌표 설정
    /// </summary>
    public void Initialize(Vector2 target)
    {
        targetPos = target;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // 목표 근접 시 폭발
        if (Vector2.Distance(transform.position, targetPos) <= arriveThreshold)
            StartCoroutine(ExplodeRoutine());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 벽·바닥 충돌 시에도 폭발
        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        if (!initialized) yield break;
        initialized = false;

        // 물리 비활성화
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        col.enabled = false;

        if (explodeDelay > 0f)
            yield return new WaitForSeconds(explodeDelay);

        // 폭발 이펙트 생성 (null 체크)
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 범위 데미지 처리
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (var hit in hits)
        {
            var dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(50);
        }

        // 수류탄 삭제
        Destroy(gameObject);
    }

    // 디버그: 폭발 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
