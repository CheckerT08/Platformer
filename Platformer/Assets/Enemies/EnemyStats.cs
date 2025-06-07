using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float damage;
    public bool invincible;
    public float speed;
    public float wallCheckDistance;
    public float groundCheckDistance;
}
