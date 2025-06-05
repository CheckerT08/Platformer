using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    public override void Awake()
    {
        base.Awake();
    }

    protected override void OnTick()
    {
        // Bewegung
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Position für Raycasts
        Vector2 origin = transform.position;
        Vector2 wallCheckDir = Vector2.right * direction;
        Vector2 groundCheckPos = origin + new Vector2(direction * 0.5f, 0);

        // Wand-Check
        bool hitWall = Physics2D.Raycast(origin, wallCheckDir, wallCheckDistance, groundLayer);

        // Klippen-Check
        bool hasGround = Physics2D.Raycast(groundCheckPos, Vector2.down, groundCheckDistance, groundLayer);

        if (hitWall || !hasGround)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("Slime explodes into goo!");
    }
}
