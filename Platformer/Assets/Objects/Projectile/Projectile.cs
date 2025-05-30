using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    private ProjectileData dat;
    private float timeAlive;
    private Vector2 velocity;
    private int piercesLeft;
    private LayerMask targetMask;
    private float dir;

    public void Setup(ProjectileData data, float direction)
    {
        dat = data;
        piercesLeft = dat.pierce;
        dir = direction;        
        velocity = new Vector2(dat.initialVelocity.x * dir, dat.initialVelocity.y);

    }

    private void Awake()
    {
        Debug.Log("dat: " + dat);
        Debug.Log("Dir: " + dir);
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (LayerMaskContainsLayer(targetMask, collision.gameObject.layer))
            //return;
        HandleCollision(collision);
    }

    void HandleCollision(Collider2D collider)
    {
        Debug.Log($"Hit: {collider.name}");
        //Destroy(gameObject);
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

    public static bool LayerMaskContainsLayer(LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
