using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Projectile")]
public class ProjectileData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float lifetime;
    public float damage;
    public int pierce;
    public LayerMask targetMask;
    public Vector2 initialVelocity;
    public Vector2 gravity;
    public Effect[] effects;
}
