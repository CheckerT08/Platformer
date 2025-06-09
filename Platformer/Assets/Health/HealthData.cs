using UnityEngine;

[CreateAssetMenu(menuName = "Health/HealthData")]
public class HealthData : ScriptableObject
{
    public float maxHealth;
    public bool canRegenerate;
    public float regenCooldown;
    public float regenAmount;
    public float regenMultiplier;
}
