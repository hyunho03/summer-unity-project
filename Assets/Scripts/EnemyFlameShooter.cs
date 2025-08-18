using System.Collections;
using UnityEngine;

public class EnemyFlameShooter : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // 플레이어 Transform
    public Transform mouth;               // 입 위치 (빈 오브젝트)
    public FlameBullet2D bulletPrefab;    // 불꽃 Prefab (FlameBullet2D 스크립트 붙은 것)
    
    [Header("Movement")]
    public float moveSpeed = 2f;          // Enemy 이동 속도

    [Header("Shooting")]
    public float interval = 3f;           // 발사 주기 (초)
    public float startDelay = 1f;         // 첫 발사 지연

    private SpriteRenderer sr;
    private Vector3 mouthDefaultLocalPos; // 입의 원래 위치 저장

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (mouth != null)
            mouthDefaultLocalPos = mouth.localPosition; // 시작 위치 기억
    }

    void Start()
    {
        StartCoroutine(ShootLoop());
    }

    void Update()
    {
        if (!player) return;

        // 🔹 플레이어 따라가기
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        // 🔹 방향 전환 (플레이어 위치에 따라 flipX)
        if (player.position.x < transform.position.x)
        {
            sr.flipX = true;   // 왼쪽 바라보기
            if (mouth != null)
                mouth.localPosition = new Vector3(-Mathf.Abs(mouthDefaultLocalPos.x), mouthDefaultLocalPos.y, mouthDefaultLocalPos.z);
        }
        else
        {
            sr.flipX = false;  // 오른쪽 바라보기
            if (mouth != null)
                mouth.localPosition = new Vector3(Mathf.Abs(mouthDefaultLocalPos.x), mouthDefaultLocalPos.y, mouthDefaultLocalPos.z);
        }
    }

    IEnumerator ShootLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(interval);
        }
    }

    void Shoot()
    {
        if (!player || !bulletPrefab || !mouth) return;

        // 🔹 발사 방향 = Enemy → Player 벡터
        Vector2 dir = (player.position - mouth.position).normalized;

        // 🔹 총알 생성
        FlameBullet2D bullet = Instantiate(bulletPrefab, mouth.position, Quaternion.identity);
        bullet.Init(dir);
    }
}
