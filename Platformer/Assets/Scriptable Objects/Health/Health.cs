using UnityEngine;

public class Health : MonoBehaviour
{
    public HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;
    private float regenCycleCooldown;

    public virtual void Start()
    {
        currentHealth = data.maxHealth;
    }

    // Methode zum Schaden nehmen
    public virtual void TakeDamage(float damageAmount)
    {
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;
        Debug.Log("Took " + damageAmount + " damage. Current health: " + currentHealth);

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
        regenCycleCooldown -= Time.deltaTime;

        if (timeSinceDamageTaken > data.regenCooldown && regenCycleCooldown < 0f)
        {
            regenCycleCooldown = 1f;
            Heal(data.regenAmount);
        }
    }

    // Methode für den Tod des Spielers
    public virtual void Die()
    {
        Debug.Log("Player died.");
    }
}
