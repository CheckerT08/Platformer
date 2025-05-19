using UnityEngine;
using System.Collections;

public abstract class Attack : ScriptableObject
{
    public string attackName;
    public float playerCooldown;
    public float castTime;
    public float damage;
    public Effect[] effects;

    public abstract IEnumerator Execute(Transform attacker, LayerMask targetLayer);
}
