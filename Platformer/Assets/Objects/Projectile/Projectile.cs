public class Projectile : MonoBehaviour
{
    private ProjectileData dat;
    private float timeAlive;

    public void SetupFromData(ProjectileData data)
    {
        dat = data;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        timeAlive += deltaTime;

        if (timeAlive > dat.lifetime)
        {
            // Death effect
            Destroy(gameObject);
            return;
        }

        velocity += gravity * deltaTime;
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
}
