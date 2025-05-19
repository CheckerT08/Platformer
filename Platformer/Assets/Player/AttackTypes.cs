using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/MeleeAttack")]
public class MeleeAttack : Attack
{
    public float hitInterval = 0.2f;

    public override IEnumerator Execute(Transform attacker, LayerMask targetLayer)
    {
        for (int i = 0; i < hitRepeatAmount; i++)
        {
            Vector2 hitPosition = (Vector2)attacker.position + offset;
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(hitPosition, range, 0, targetLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                // Damage the enemy
                if (enemy.TryGetComponent(out Health health))
                    health.TakeDamage(damage);

                // Apply Effects
                foreach (var effect in effects)
                {
                    if (enemy.TryGetComponent(out StatusEffectReceiver receiver))
                        receiver.ApplyEffect(effect);
                }
            }

            if (hitRepeatAmount > 1)
                yield return new WaitForSeconds(hitInterval);
        }
    }
}


[CreateAssetMenu(menuName = "Attacks/Ranged")]
public class RangedAttack : Attack
{
    public GameObject projectilePrefab;
    public float projectileSpeed;

    public override void Execute(Transform attacker, Vector2 direction)
    {
        GameObject proj = GameObject.Instantiate(projectilePrefab, attacker.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(direction, damage, projectileSpeed);
    }
}
