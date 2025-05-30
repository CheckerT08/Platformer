using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee/Axe")]
public class AxeAttack : MeleeAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }
}

