using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public float hitInterval;
    public int hitRepeatAmount;
    public Vector2 offset;
    public Vector2 range;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield break;
    }
}

[CreateAssetMenu(menuName = "Attacks/Melee/Sword")]
public class SwordAttack : RangedAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }
}

[CreateAssetMenu(menuName = "Attacks/Melee/Axe")]
public class AxeAttack : RangedAttack
{
    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        yield return null;
    }
}


/*
        for (int i = 0; i < hitRepeatAmount; i++)
        {
            Vector2 hitPosition = (Vector2)attacker.position + offset;
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(hitPosition, range, 0, targetLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                // Damage the enemy

                // Apply Effects
                /*foreach (var effect in effects)
                {
                    if (enemy.TryGetComponent(out StatusEffectReceiver receiver))
                        receiver.ApplyEffect(effect);
                }
            }

            if (hitRepeatAmount > 1)
                yield return new WaitForSeconds(hitInterval);
        }
        */
