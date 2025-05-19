using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/MeleeAttack")]
public class MeleeAttack : Attack
{
    public float hitInterval;
    public float hitRepeatAmount;
    public Vector2 offset;
    public Vector2 range;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Melee " + hitRepeatAmount);
        yield return null;

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

    }
}
