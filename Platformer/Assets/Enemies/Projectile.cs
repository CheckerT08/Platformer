using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float lifetime = 5f;
    private float timeAlive = 0f;
    private int pierce;
    private LayerMask targetMask;
    private Vector2 velocity;
    private Vector2 gravity;
    private Effect[] effects;

    public void Init(LayerMask toHit, Vector2 vel, Vector2 grav, int pier, Effect[] eff)
    {
        targetMask = toHit;
        velocity = vel;
        gravity = grav;
        pierce = pier;
        effects = eff;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        timeAlive += deltaTime;

        if (timeAlive > lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Apply gravity
        velocity += gravity * deltaTime;

        // Calculate movement
        Vector2 moveDist = velocity * deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, moveDist.magnitude, targetMask);

        if (hit.collider != null)
        {
            HandleCollision(hit.collider);
            if (pierce == 0)
                Destroy(gameObject);
            else
                pierce--;
        }

        transform.position += (Vector3)moveDist;
    }

    void HandleCollision(Collider2D collider)
    {
        Debug.Log($"Hit: {collider.name}");
        // TODO: Damage enemy and apply effects
    }

    public void Deflect(Vector2 newDirection, LayerMask mask)
    {
        velocity = newDirection;
        targetMask = mask;
        Debug.Log($"Projectile deflected to direction: {velocity}");
    }

    public void SetGravity(Vector2 newGravity)
    {
        gravity = newGravity;
    }
}
