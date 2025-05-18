using UnityEngine;
using System;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private LayerMask enemieLayer;
    [SerializeField] private Attack[] attacks;

    private bool isAttacking;
    private float cooldown;
    
    private void Attack(string attackName)
    {
        Attack attack = attacks.FirstOrDefault(attack => attack.name == attackName);
        if (attack == null) return;
        if (isAttacking) return;
        isAttacking = true;
        
        PerformAttack(attack);
        
        cooldown = attack.cooldown;
    }

    private IEnumerator PerformAttack(Attack attack)
    {
        for (int i = 0; i < attack.hitRepeatAmount; i++)
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll((Vector2)player.transform.position + attack.offset, attack.range, 0, enemieLayer);
            foreach (Collider2D enemy in enemiesToDamage)
            {
                // Damage Enemie
                
                foreach (Effect effect in attack.effects)
                {
                    // Apply Effect
                }
            }
            yield return new WaitForSeconds(1f); // Cooldown between hits
        }

        isAttacking = false;
        cooldown = attack.cooldown; // After attack cooldown
    }

}

[Serializable]
public class Attack
{
    public string name;
    public float cooldown;
    public float castTime;
    public float damage;
    public int hitRepeatAmount;
    public Vector2 range;
    public Vector2 offset;
    public Effect[] effects;
}

[Serializable]
public class Effect
{
    public EffectW effect;
    public float duration;
}


public enum EffectW
{
    Fire,
    Poison,
    Bleed,
    
    Speed,
    Slowness,
    Haste,
    AntiHaste,
    
    Weakness,
    Strength,
    AttackWeakness,
    AttackStrength
}
