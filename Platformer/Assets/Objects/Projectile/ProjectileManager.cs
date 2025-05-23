using UnityEngine;

public static class ProjectileManager
{
    public static void Spawn(ProjectileData data, Vector2 position, Quaternion rotation)
    {
        if (data.projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned in ProjectileData!");
            return;
        }

        GameObject instance = GameObject.Instantiate(data.projectilePrefab, position, rotation);
        Projectile projectile = instance.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Setup(data);
        }
        else
        {
            Debug.LogError("Projectile prefab does not contain a Projectile component!");
        }
    }
}
