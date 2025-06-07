using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    protected override void OnTick()
    {
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (CheckWall() || CheckCliff())
            Flip();
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Slime explodes into goo!");
    }
}
