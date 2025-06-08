using UnityEngine;
using System.Collections;

public class MeleeAttack : AttackBase
{
    public float damage;    
    public float hitInterval;
    public Effect[] atteffects;
    public int hitRepeatAmount;
    public Vector2 offset;
    public Vector2 range;

    public override sealed IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        Debug.Log("Melee");
        yield return BeforeAttack(attacker, targetLayer);

        for (int i = 0; i < hitRepeatAmount; i++)
        {
            yield return BeforeHit(attacker, targetLayer);

            Vector2 hitPosition = (Vector2)attacker.position + (offset * attacker.localScale.x);
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(hitPosition, range, 0, targetLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                Game.Damager.Damage(enemy.gameObject, damage);

                foreach (Effect effect in atteffects)
                {
                    effect.ApplyTo(enemy.gameObject);
                }

                yield return OnHit(attacker, targetLayer, enemy);
            }

            yield return AfterHit(attacker, targetLayer);

            if (hitRepeatAmount > 1)
                yield return new WaitForSeconds(hitInterval);
        }

        yield return AfterAttack(attacker, targetLayer);
    }

    // Virtuelle Erweiterungspunkte
    protected virtual IEnumerator BeforeAttack(Transform attacker, LayerMask targetLayer) => null;
    protected virtual IEnumerator BeforeHit(Transform attacker, LayerMask targetLayer) => null;
    protected virtual IEnumerator OnHit(Transform attacker, LayerMask targetLayer, Collider2D hit) => null;
    protected virtual IEnumerator AfterHit(Transform attacker, LayerMask targetLayer) => null;
    protected virtual IEnumerator AfterAttack(Transform attacker, LayerMask targetLayer) => null;
}
