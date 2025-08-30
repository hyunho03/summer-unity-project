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
    [SerializeField] private Transform character;
    [SerializeField] private float orbitRadius = 1f;

    // ====== 🔊 SFX (무기에서 재생) ======
    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;     // 무기/플레이어에 붙인 AudioSource
    [SerializeField] private AudioClip shootClip;       // 총소리
    [Range(0f,1f)] [SerializeField] private float shootVolume = 1f;
    [SerializeField] private Vector2 randomPitch = new Vector2(0.95f, 1.05f);

    private Camera mainCam;
    private float cooldownTimer;

    void Awake()
    {
        mainCam = Camera.main;

        // 인스펙터에서 비워두면 자동으로 가져와서 2D SFX 세팅
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;   // 2D로 재생
        }
    }

    void Update()
    {
        // 1) 마우스 화면 → 월드 좌표
        Vector3 ms = Input.mousePosition;
        ms.z = Mathf.Abs(mainCam.transform.position.z - character.position.z);
        Vector3 mw = mainCam.ScreenToWorldPoint(ms);

        // 2) 캐릭터 → 마우스 방향
        Vector2 dir = ((Vector2)mw - (Vector2)character.position).normalized;

        // 3) 궤도 중심
        Vector2 orbitCenter = (Vector2)character.position + Vector2.up * 0.5f;
        transform.position = orbitCenter + dir * orbitRadius;

        // 4) 총 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5) 좌우 반전
        Vector3 s = transform.localScale;
        s.y = dir.x < 0f ? -Mathf.Abs(s.y) : Mathf.Abs(s.y);
        transform.localScale = s;

        // 6) 연사
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
            rb.linearVelocity = firePoint.right * bulletSpeed; // 네 프로젝트 규격 유지

        if (b.TryGetComponent<Bullet>(out var bullet))
            bullet.damage = bulletDamage;
        else
            Debug.LogWarning("Bullet prefab missing Bullet component.");

        // 🔊 무기에서 총소리 재생 (총알이 파괴돼도 끝까지 재생됨)
        if (sfxSource != null && shootClip != null)
        {
            sfxSource.pitch = Random.Range(randomPitch.x, randomPitch.y);
            sfxSource.PlayOneShot(shootClip, shootVolume);
        }
    }

    public void IncreaseBulletDamage(int amount)
    {
        bulletDamage += amount;
        Debug.Log("총알 데미지 증가! 현재 데미지: " + bulletDamage);
    }

    public void DecreaseReroading(float percent)
    {
        fireCooldown *= (1f - percent);
        Debug.Log("장전시간 감소! 현재 장전시간: " + fireCooldown);
    }
}
