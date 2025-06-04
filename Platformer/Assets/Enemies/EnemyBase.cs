using UnityEngine;

public class EnemyBase : Health
{
    [Header("Enemy Configuration")]
    public EnemyStats enemyStats;

    protected float currentHealth;
    protected bool isInvincible;

    public virtual void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("No EnemyStats assigned to " + gameObject.name);
            return;
        }

        currentHealth = enemyStats.maxHealth;
        isInvincible = enemyStats.invincible;
    }

    public override void TakeDamage(float amount)
    {
        return;
    }

    public override void Die()
    {
        return;
    }
}
