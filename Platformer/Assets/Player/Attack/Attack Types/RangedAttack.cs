using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/RangedAttack")]
public class RangedAttack : Attack
{
    public float velocity;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log($"Ranged Attack, velocity: {velocity}");
        yield return null;
    }
}
