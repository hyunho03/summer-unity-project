using UnityEngine;

public class SwordFollowDirection : MonoBehaviour
{
    private SpriteRenderer sr;
    private SpriteRenderer bossSR;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        bossSR = GetComponentInParent<SpriteRenderer>(); // 부모 보스 SR 참조
    }

    void Update()
    {
        if (bossSR != null)
        {
            // 🔹 보스 flipX 값 그대로 따라가기
            sr.flipX = bossSR.flipX;
        }
    }
}
