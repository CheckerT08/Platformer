using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;
    private float regenCycleCooldown;
    private bool canRegen;


    private void Awake()
    {
        currentHealth = data.maxHealth;
        canRegen = data.canRegenerate;
    }

    public void TakeDamage(float damageAmount)
    {
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float healAmount)
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

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}