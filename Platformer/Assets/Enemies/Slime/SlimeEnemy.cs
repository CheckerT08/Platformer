using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    private float direction => movingRight ? 1f : -1f;

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (CheckWall())
            Flip();
    }

    protected override void OnTick()
    {
        
    }

    public override float GetXInput()
    {
        return 0f;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Slime explodes into goo!");
    }
}
