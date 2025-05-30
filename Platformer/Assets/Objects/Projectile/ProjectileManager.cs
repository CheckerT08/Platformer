using UnityEngine;

public static class ProjectileManager
{
    public static void Spawn(ProjectileData data, Vector2 position, Quaternion rotation, float direction)
    {
        if (data.projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned in ProjectileData!");
            return;
        }

        GameObject instance = GameObject.Instantiate(data.projectilePrefab, position, rotation);
        if (instance.TryGetComponent(out Projectile projectile))
            projectile.Setup(data, direction);
    }
}
