using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Walking : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;

    [Header("Idle Settings")]
    [Tooltip("Seconds to wait before playing Bored/Yawn")]
    public float idleThreshold = 3f;    // 5f → 3f로 변경

    private Vector2 move;
    private float idleTimer = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 0) Y 키 눌렀을 때 즉시 Bored/Yawn 랜덤 실행
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (Random.value < 0.5f)
                animator.SetTrigger("bored");
            else
                animator.SetTrigger("yawn");
            idleTimer = 0f;
            return; // Y키만 누른 프레임에는 이동/idle 로직을 스킵해도 좋습니다.
        }

        // 1) WASD 입력 처리
        move = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector2.left;
        if (Input.GetKey(KeyCode.D)) move += Vector2.right;
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;
        move = move.normalized;

        // 2) 스프라이트 좌우 반전
        if (move.x < 0f) spriteRenderer.flipX = true;
        else if (move.x > 0f) spriteRenderer.flipX = false;

        // 3) 이동 중 vs 정지 중 분기
        if (move.magnitude > 0f)
        {
            animator.SetTrigger("move");
            idleTimer = 0f;  // 다시 움직였으니 idle 타이머 리셋
        }
        else
        {
            animator.SetTrigger("stop");
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold)
            {
                // 50% 확률로 Bored or Yawn
                if (Random.value < 0.5f)
                    animator.SetTrigger("bored");
                else
                    animator.SetTrigger("yawn");

                idleTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
