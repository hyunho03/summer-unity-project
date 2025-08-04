using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Explosion : MonoBehaviour
{
    void Start()
    {
        // Animator에서 첫 번째 애니메이션 클립의 길이를 가져와서
        // 그 길이만큼 지난 뒤에 이 게임오브젝트를 파괴합니다.
        Animator anim = GetComponent<Animator>();
        if (anim.runtimeAnimatorController != null
            && anim.runtimeAnimatorController.animationClips.Length > 0)
        {
            // 단일 클립만 있다고 가정. 여러 개라면 이름으로 찾으세요.
            float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
            Destroy(gameObject, clipLength);
        }
        else
        {
            // 안전장치: 컨트롤러나 클립이 없으면 기본 0.5초 후 파괴
            Debug.LogWarning("Explosion: Animator Controller 또는 Clips가 없습니다.");
            Destroy(gameObject, 0.5f);
        }
    }
}
