using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    public override void Awake()
    {
        base.Awake();
    }

    protected override void OnTick()
    {
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("Slime explodes into goo!");
    }
}
