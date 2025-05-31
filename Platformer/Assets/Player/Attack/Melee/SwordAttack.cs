using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee/Sword")]
public class SwordAttack : MeleeAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Sword Attack");
        yield return null;
    }
}
