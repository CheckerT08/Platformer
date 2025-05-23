using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public EnemyStats enemyStats;

    protected float currentHealth;
    protected bool isInvincible;

    protected virtual void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("No EnemyStats assigned to " + gameObject.name);
            return;
        }

        currentHealth = enemyStats.maxHealth;
        isInvincible = enemyStats.invincible;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isInvincible)
        {
            Debug.Log($"{name} is invincible and took no damage.");
            return;
        }

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void ApplyEffect(Effect[] effects)
    {
        foreach (var effect in effects)
        {
            // Apply effect
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
        Destroy(gameObject);
    }
}
