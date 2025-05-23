using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float maxHealth;
    public float damage;
    public bool invincible;
}
