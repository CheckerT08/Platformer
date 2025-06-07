using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public HealthData data;
    private float currentHealth;
    private float timeSinceDamageTaken;
    private float regenCycleCooldown;
    private bool canRegen;

    private Renderer rend;
    private Color originalColor;
    private bool isFlashing;

    private void Awake()
    {
        currentHealth = data.maxHealth;
        canRegen = data.canRegenerate;

        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    public void TakeDamage(float damageAmount)
    {
        timeSinceDamageTaken = 0f;
        currentHealth -= damageAmount;

        if (!isFlashing) StartCoroutine(Flash());

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

    private System.Collections.IEnumerator Flash()
    {
        isFlashing = true;
        if (rend != null)
        {
            rend.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            rend.material.color = originalColor;
        }
        isFlashing = false;
    }
}
