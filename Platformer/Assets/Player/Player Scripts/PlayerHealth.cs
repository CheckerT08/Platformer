using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerHealth : MonoBehaviour, Damageable
{
    Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    public void TakeDamage(float amount)
    {
        health.ReduceHealth(amount);
    }
}