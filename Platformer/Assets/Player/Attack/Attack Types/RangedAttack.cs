using System.Collections;
using UnityEngine;

public class RangedAttack : Attack
{
    public float velocity;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield break;
    }
}

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

[CreateAssetMenu(menuName = "Attacks/Ranged/IceArrow")]
public class IceArrowAttack : RangedAttack
{
    public GameObject iceArrowPrefab;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        // IceArrow-spezifische Logik
        yield return null;
    }
}
