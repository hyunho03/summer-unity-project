using UnityEngine;

public class GunFire : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Damage Settings")]
    [SerializeField] private int bulletDamage = 3;

    [Header("Fire Rate")]
    [SerializeField] private float fireCooldown = 0.2f;

    [Header("Orbit Settings")]
    [SerializeField] private Transform character;    // 궤도 중심이 될 캐릭터
    [SerializeField] private float orbitRadius = 1f; // 캐릭터로부터의 거리

    private Camera mainCam;
    private float cooldownTimer;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1) 마우스 화면 → 월드 좌표
        Vector3 ms = Input.mousePosition;
        ms.z = Mathf.Abs(mainCam.transform.position.z - character.position.z);
        Vector3 mw = mainCam.ScreenToWorldPoint(ms);

        // 2) 캐릭터 → 마우스 방향 벡터
        Vector2 dir = ((Vector2)mw - (Vector2)character.position).normalized;

        // 3) 궤도 중심을 캐릭터 위치 위로 0.5만큼 고정
        Vector2 orbitCenter = (Vector2)character.position + Vector2.up * 0.5f;
        transform.position = orbitCenter + dir * orbitRadius;

        // 4) 총 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5) 수직축 기준 좌측 반원일 때만 상하 반전
        Vector3 s = transform.localScale;
        s.y = dir.x < 0f
            ? -Mathf.Abs(s.y)
            : Mathf.Abs(s.y);
        transform.localScale = s;

        // 6) 연속 발사 처리
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && cooldownTimer <= 0f)
        {
            Fire();
            cooldownTimer = fireCooldown;
        }
    }

    private void Fire()
    {
        var b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (b.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = firePoint.right * bulletSpeed;

        if (b.TryGetComponent<Bullet>(out var bullet))
            bullet.damage = bulletDamage;
        else
            Debug.LogWarning("Bullet prefab missing Bullet component.");
    }

    public void IncreaseBulletDamage(int amount)
    {
        bulletDamage += amount;
        Debug.Log("총알 데미지 증가! 현재 데미지: " + bulletDamage);
    }
    public void DecreaseReroading(float percent)
    {
        fireCooldown*= (1f - percent);
        Debug.Log("장전시간 감소! 현재 장전시간: " + fireCooldown);
    }

}
