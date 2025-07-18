using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    private float inputX = 1f;

    protected override void Awake()
    {
        base.Awake();
        body.OnFlip += () => inputX = -inputX;
    }

    private void Update()
    {
        if (body.IsWall()) body.Flip();
    }

    protected override void OnTick()
    {
        
    }

    protected override void OnPlayerHit(Collider2D collision)
    {
        base.OnPlayerHit(collision);
    }

    public override float GetXInput()
    {
        return inputX;
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Slime explodes into goo!");
    }
}