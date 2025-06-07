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

    public override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Slime explodes into goo!");
    }
}
