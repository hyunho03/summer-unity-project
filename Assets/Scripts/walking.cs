using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class walking : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 move;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        move = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
            move += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow))
            move += Vector2.right;
        if (Input.GetKey(KeyCode.UpArrow))
            move += Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow))
            move += Vector2.down;

        move = move.normalized;

        if (move.x < 0) spriteRenderer.flipX = true;
        if (move.x > 0) spriteRenderer.flipX = false;

        if (move.magnitude > 0) animator.SetTrigger("move");
        else animator.SetTrigger("stop");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
