using UnityEngine;

public class EnemyHealth : Health
{
    public override void TakeDamage(float amt)
    {
        Debug.Log("Enemy takes damage: " + amt);
        base.TakeDamage(amt);
        // z. B. Partikel, DeathCheck, etc.
    }
}
