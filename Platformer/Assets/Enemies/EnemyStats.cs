using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Tooltip("Damage to deal to player when colliding with player")] public float damage;
    public float speed;
}
