using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeDamage(float amt)
    {
        Debug.Log("Player takes damage: " + amt);
        health?.TakeDamage(amt);
        // z.B. Kamera wackeln, UI Feedback etc.
    }
}
