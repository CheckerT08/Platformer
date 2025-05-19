[CreateAssetMenu(menuName = "Attacks/Melee")]
public class MeleeAttack : Attack
{
    public float movementMultiplier 
    public Vector2 attackSize;
    public float[] attackRepeatTimes;

    public override IEnumerator Execute(Transform attacker, Vector2 direction)
    {
      // Hit
        Collider2D[] hits = Physics2D.OverlapCircleAll(attacker.position + (Vector3)(direction * range), hitRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Health>().TakeDamage(damage);
        }
        if (effectPrefab)
            GameObject.Instantiate(effectPrefab, attacker.position, Quaternion.identity);
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
