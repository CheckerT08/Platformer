using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Methode zum Schaden nehmen
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Player took " + damageAmount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Methode für den Tod des Spielers
    private void Die()
    {
        Debug.Log("Player died.");
        // Hier kannst du Tod-Animationen, Respawn oder Game Over aufrufen
        // Zum Beispiel:
        // Destroy(gameObject); // falls der Spieler entfernt werden soll
    }
}
