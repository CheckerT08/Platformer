using UnityEngine;

public class Health : MonoBehaviour
{
    public HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;

    virtual void Start()
    {
        currentHealth = data.maxHealth;
    }

    // Methode zum Schaden nehmen
    public virtual void TakeDamage(float damageAmount)
    {
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;
        Debug.Log("Player took " + damageAmount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Regen Logik

    // Methode für den Tod des Spielers
    private virtual void Die()
    {
        Debug.Log("Player died.");
    }
}
