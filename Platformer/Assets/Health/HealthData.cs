using UnityEngine;

[CreateAssetMenu(menuName = "Health Data")]
public class HealthData : ScriptableObject
{
    public float maxHealth;
    [Space]
    public bool canRegenerate;
    public float timeFromDamageToHeal;
    public float regenCycleCooldown;
    public float regenAmount;
    [Space]
    public bool invincible;
}
