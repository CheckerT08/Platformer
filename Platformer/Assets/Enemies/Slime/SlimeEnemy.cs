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

    public override float GetXInput()
    {
        return inputX;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Slime explodes into goo!");
    }
}
