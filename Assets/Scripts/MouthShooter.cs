// MouthShooter2D.cs
using System.Collections;
using UnityEngine;

public class MouthShooter2D : MonoBehaviour
{
    public Transform mouth;                // MouthSocket
    public FlameBullet2D bulletPrefab;     // 위에서 만든 프리팹
    public Transform target;               // 플레이어 Transform (없으면 전방으로)
    public float interval = 2f;            // 발사 주기
    public float startDelay = 0.5f;        // 시작 지연

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            ShootOnce();
            yield return new WaitForSeconds(interval);
        }
    }

    public void ShootOnce() // 애니메이션 이벤트에서도 호출 가능
    {
        var b = Instantiate(bulletPrefab, mouth.position, Quaternion.identity);
        Vector2 dir = target 
            ? (target.position - mouth.position) 
            : (transform.localScale.x >= 0 ? Vector2.right : Vector2.left);
        b.Init(dir);
    }
}
