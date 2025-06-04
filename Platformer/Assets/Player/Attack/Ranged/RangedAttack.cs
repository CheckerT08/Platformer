using System.Collections;
using UnityEngine;

public class RangedAttack : AttackBase
{
    public ProjectileData projectile;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield break;
    }
}