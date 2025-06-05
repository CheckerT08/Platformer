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

        // Flip bei Wand oder Klippe
        if (CheckWall() || CheckCliff())
            Flip();
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
