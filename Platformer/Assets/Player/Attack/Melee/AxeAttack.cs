using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee/Axe")]
public class AxeAttack : MeleeAttack
{
    protected override IEnumerator BeforeHit(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }

    protected override IEnumerator AfterHit(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }
}
