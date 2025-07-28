using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Walking : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;

    [Header("Idle Settings")]
    [Tooltip("Seconds to wait before playing Bored/Yawn")]
    public float idleThreshold = 3f;    // 5f �� 3f�� ����

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
        // 0) Y Ű ������ �� ��� Bored/Yawn ���� ����
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (Random.value < 0.5f)
                animator.SetTrigger("bored");
            else
                animator.SetTrigger("yawn");
            idleTimer = 0f;
            return; // YŰ�� ���� �����ӿ��� �̵�/idle ������ ��ŵ�ص� �����ϴ�.
        }

        // 1) WASD �Է� ó��
        move = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector2.left;
        if (Input.GetKey(KeyCode.D)) move += Vector2.right;
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;
        move = move.normalized;

        // 2) ��������Ʈ �¿� ����
        if (move.x < 0f) spriteRenderer.flipX = true;
        else if (move.x > 0f) spriteRenderer.flipX = false;

        // 3) �̵� �� vs ���� �� �б�
        if (move.magnitude > 0f)
        {
            animator.SetTrigger("move");
            idleTimer = 0f;  // �ٽ� ���������� idle Ÿ�̸� ����
        }
        else
        {
            animator.SetTrigger("stop");
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold)
            {
                // 50% Ȯ���� Bored or Yawn
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
