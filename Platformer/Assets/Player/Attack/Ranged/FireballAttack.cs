using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged/Fireball")]
public class FireballAttack : RangedAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Fireball Attack");
        ProjectileManager.Spawn(data, attacker.position, attacker.rotation, attacker.localScale.x);
        yield return null;
    }
}
