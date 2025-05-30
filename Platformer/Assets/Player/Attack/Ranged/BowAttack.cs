using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged/Bow")]
public class BowAttack : RangedAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Bow Attack");
        ProjectileManager.Spawn(data, attacker.position, attacker.rotation, attacker.localScale.x); 
        yield return null;
    }
}
