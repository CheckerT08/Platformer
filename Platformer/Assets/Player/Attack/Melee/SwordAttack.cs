using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee/Sword")]
public class SwordAttack : MeleeAttack
{
    protected override IEnumerator BeforeHit(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }

    protected override IEnumerator AfterHit(Transform attacker, LayerMask targetLayer)
    {
        // z. B. Partikel/Sound
        yield return null;
    }
}
