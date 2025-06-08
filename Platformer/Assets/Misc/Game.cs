using UnityEngine;

public static class Game
{
    public static class Layer
    {
        public static readonly LayerMask groundMask = LayerMask.GetMask("Level Collidable");
        public static readonly LayerMask enemyMask = LayerMask.GetMask("Enemies");
        public static readonly LayerMask ladderMask = LayerMask.GetMask("Level Ladder");

        public static bool LayerMaskContainsLayer(LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }

    public static class Damager
    {
        public static void Damage(GameObject obj, float damage)
        {
            obj.TryGetComponent<IDamageable>(out var target);
            target?.TakeDamage(damage);
        }
    }
}
