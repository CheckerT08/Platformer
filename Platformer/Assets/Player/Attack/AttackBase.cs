using UnityEngine;
using System.Collections;

public abstract class AttackBase : ScriptableObject
{
    public string attackName;
    public float playerCooldown;
    public float castTime;

    public abstract IEnumerator Execute(Transform attacker, LayerMask targetLayer);
}
