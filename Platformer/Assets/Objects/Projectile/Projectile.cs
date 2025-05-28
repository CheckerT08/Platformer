using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    private ProjectileData dat;
    private float timeAlive;
    private Vector2 velocity;
    private int piercesLeft;
    private LayerMask targetMask;

    private HashSet<Collider2D> piercedTargets = new HashSet<Collider2D>();
    private HashSet<Collider2D> currentFrameHits = new HashSet<Collider2D>();

    public void Setup(ProjectileData data)
    {
        dat = data;
        piercesLeft = dat.pierce;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        timeAlive += deltaTime;

        if (timeAlive > dat.lifetime)
        {
            Destroy(gameObject);
            return;
        }

        velocity += dat.gravity * deltaTime;
        Vector2 moveDist = velocity * deltaTime;
        transform.position += (Vector3)moveDist;

        // Berechne Kollisionsabfrage an der aktuellen Position
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.05f, targetMask);

        currentFrameHits.Clear();

        foreach (var hit in hits)
        {
            currentFrameHits.Add(hit);

            if (!piercedTargets.Contains(hit))
            {
                piercedTargets.Add(hit);
                HandleCollision(hit);

                if (piercesLeft == 0)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    piercesLeft--;
                }
            }
        }

        // Entferne verlassene Collider
        piercedTargets.RemoveWhere(col => !currentFrameHits.Contains(col));
    }

    void HandleCollision(Collider2D collider)
    {
        Debug.Log($"Hit: {collider.name}");
        // TODO: Damage enemy and apply effects
        // if (targetLayer == enemy) collider.GetComponent<EnemyBase>().TakeDamage(dat.damage);
        // if (targetLayer == player) collider.GetComponent<PlayerHealth>().TakeDamage(dat.damage);
    }

    public void Deflect(Vector2 newDirection, LayerMask mask)
    {
        velocity = newDirection;
        targetMask = mask;
        Debug.Log($"Projectile deflected to direction: {velocity}");
    }
}
