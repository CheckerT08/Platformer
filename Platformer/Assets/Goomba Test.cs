using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SimpleEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public Vector2 gravity = new Vector2(0f, -9.81f);
    public LayerMask groundMask;
    public Transform groundCheck;
    public Transform wallCheck;

    private Vector2 velocity;
    private bool facingRight = false;
    private bool grounded = false;

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Apply gravity
        velocity += gravity * deltaTime;

        // Move horizontally
        float dir = facingRight ? 1f : -1f;
        Vector2 horizontalMove = new Vector2(dir * moveSpeed, 0f);
        transform.position += (Vector3)(horizontalMove * deltaTime);

        // Move vertically
        transform.position += (Vector3)(velocity * deltaTime);

        // Check for ground and wall
        grounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundMask);
        bool hitWall = Physics2D.Raycast(wallCheck.position, facingRight ? Vector2.right : Vector2.left, 0.1f, groundMask);
        bool ledgeAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundMask);

        if (hitWall || !ledgeAhead)
        {
            Flip();
        }

        // Reset vertical velocity if grounded
        if (grounded && velocity.y < 0f)
        {
            velocity.y = 0f;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public void Die()
    {
        // TODO: Animation, Sound, Score, etc.
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.GetComponent<Projectile>();
        if (projectile != null)
        {
            Die();
        }
    }
}
