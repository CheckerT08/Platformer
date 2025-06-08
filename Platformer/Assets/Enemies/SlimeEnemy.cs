using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    private float direction => movingRight ? 1f : -1f;

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (CheckWall())
            Flip();
        if (CheckWall()) print("Wall");

    }

    protected override void OnTick()
    {
        
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Slime explodes into goo!");
    }
}
