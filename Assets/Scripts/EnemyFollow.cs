using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        // ➤ 왼쪽/오른쪽 반전
        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;
    }
}
