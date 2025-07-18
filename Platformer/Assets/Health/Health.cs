using UnityEngine;

public class Health : MonoBehaviour
{
    public HealthData dataPublic;
    private HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;
    private float timeSinceLastHealed;
    private bool canRegen;


    private void Awake()
    {
        data = Instantiate(dataPublic);
        currentHealth = data.maxHealth;
        canRegen = data.canRegenerate;
        if (gameObject.TryGetComponent(out BoxCollider2D coll)) 
            coll.enabled = !data.invincible;
    }

    public virtual void ReduceHealth(float damageAmount)
    {
        if (data.invincible) return;

        Debug.Log($"{gameObject.name} took {damageAmount} damage.");
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
            Die();
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, data.maxHealth);
        Debug.Log("Healed " + healAmount + " hp. Current health: " + currentHealth);
    }

    private void Update()
    {
        timeSinceDamageTaken += Time.deltaTime;
        if (canRegen)
        {
            timeSinceLastHealed += Time.deltaTime;
            if (timeSinceDamageTaken > data.timeFromDamageToHeal && timeSinceLastHealed > data.regenCycleCooldown)
            {
                timeSinceLastHealed = 0f;
                Heal(data.regenAmount);
            }
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}