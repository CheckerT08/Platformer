using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private ProjectileData dat;
    private float timeAlive;
    private Vector2 velocity;
    private int piercesLeft;
    private LayerMask targetMask;
    private float dir;
    SpriteRenderer sr;

    public void Setup(ProjectileData data, float direction)
    {
        dat = data;
        piercesLeft = dat.pierce;
        dir = direction;
        velocity = new Vector2(dat.initialVelocity.x * direction, dat.initialVelocity.y);
        targetMask = dat.targetMask;
        GetComponent<Collider2D>().includeLayers = targetMask;
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = dir < 0;
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
        if (timeAlive < 0.1) return;
        if (!LayerMaskContainsLayer(targetMask, collision.gameObject.layer)) return;
        HandleCollision(collision);
    }

    void HandleCollision(Collider2D collider)
    {
        piercesLeft--;
        if (piercesLeft < 1)
            Destroy(gameObject);

        if (collider.TryGetComponent(out Health health))
            health.TakeDamage(dat.damage);

        foreach (Effect effect in dat.effects)
            effect.ApplyTo(collider.gameObject);
    }

    public void Deflect(Vector2 newVelocity, LayerMask mask)
    {
        velocity = newVelocity;
        dir = Mathf.Sign(newVelocity.x);
        sr.flipX = dir < 0;
        targetMask = mask;
        Debug.Log($"Projectile deflected to direction: {velocity}");
    }

    public static bool LayerMaskContainsLayer(LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
