using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(float amt)
    {
        Debug.Log("Player takes damage: " + amt);
        base.TakeDamage(amt);
        // zusätzliche Spielerlogik hier (z. B. Animation, UI)
    }
}
