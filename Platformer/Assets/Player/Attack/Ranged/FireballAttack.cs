using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged/Fireball")]
public class FireballAttack : RangedAttack
{
    public GameObject fireballPrefab;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        // Fireball-spezifische Logik
        yield return null;
    }
}
