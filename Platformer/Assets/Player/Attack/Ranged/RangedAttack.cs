using System.Collections;
using UnityEngine;

public class RangedAttack : AttackBase
{
    public ProjectileData data;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Ranged");
        yield break;
    }
}