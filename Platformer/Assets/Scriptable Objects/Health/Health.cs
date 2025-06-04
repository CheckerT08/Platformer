using UnityEngine;

public class Health : MonoBehaviour
{
    public HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;
    private float regenCycleCooldown;
    private bool canRegen;

    public virtual void Start()
    {
        currentHealth = data.maxHealth;
        canRegen = data.canRegenerate;
    }

    // Methode zum Schaden nehmen
    public virtual void TakeDamage(float damageAmount)
    {
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth += healAmount;
        Debug.Log("Healed " + healAmount + " hp. Current health: " + currentHealth);
    }

    private void Update()
    {
        timeSinceDamageTaken += Time.deltaTime;
        if (canRegen)
        {
            regenCycleCooldown -= Time.deltaTime;

            if (timeSinceDamageTaken > data.regenCooldown && regenCycleCooldown < 0f)
            {
                regenCycleCooldown = 1f;
                Heal(data.regenAmount);
            }
        }
    }

    // Methode für den Tod des Spielers
    public virtual void Die()
    {
        Debug.Log("Died.");
    }
}
