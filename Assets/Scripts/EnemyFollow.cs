using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyFollow : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public Transform target;
    public float speed = 2f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;
        Vector2 dir = ((Vector2)target.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} dmg ({currentHealth}/{maxHealth})");
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
