using UnityEngine;
using System;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private LayerMask enemieLayer;
    [SerializeField] private Attack[] attacks;

    private float cooldown;
    
    private void Attack(string attackName)
    {
        Attack attack = attacks.FirstOrDefault(attack => attack.name == attackName);
        if (attack == null) return;
        cooldown = 100f;
        
        // Effect
        
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
        }
        
        cooldown = attack.cooldown;
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
