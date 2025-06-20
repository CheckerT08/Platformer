using UnityEngine;
// Add: OverrideVelocity(), GetVelocity()

[RequireComponent(typeof(BoxCollider2D))]
public class MovementBody : MonoBehaviour
{
    public MovementData data;

    [Header("Collision")]
    Vector2 velocity;
    Vector2 input;
    public bool isClimbingLadder;

    const float skinWidth = .015f;
    const float dstBetweenRays = .25f;
    int horizontalRayCount, verticalRayCount;
    float horizontalRaySpacing, verticalRaySpacing;

    float velocityXSmoothing;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisions = new CollisionInfo();
        CalculateRaySpacing();
    }

    void Update()
    {
        UpdateRaycastOrigins();
        ApplyGravity();

        float targetVelocityX = inputX * speed;
        float accelTime = collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);

        gravityActive = !isClimbingLadder;

        HandleWallSlide();
        Move(velocity * Time.deltaTime);
    }

    public void SetInput(float x)
    {
        inputX = Mathf.Clamp(x, -1f, 1f);
    }

    public void Jump(float jumpForce)
    {
        if (collisions.below)
            velocity.y = jumpForce;
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void ApplyGravity()
    {
        if (!gravityActive) return;

        float gravity = velocity.y > 0 ? upGravity : downGravity;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -data.maxFallSpeed, float.MaxValue);
    }

    void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        float dirY = Mathf.Sign(moveAmount.y);
        float rayLength = 1 + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = dirY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);
            if (hit)
            {
                Debug.Log($"Has hit {hit.collider.gameObject.name}. V");
                moveAmount.y = (hit.distance - skinWidth) * dirY;
                velocity.y = 0;
                if (dirY == -1) collisions.below = true;
                else collisions.above = true;
                break;
            }
        }

        float dirX = Mathf.Sign(moveAmount.x);
        rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = dirX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);
            if (hit)
            {
                Debug.Log($"Has hit {hit.collider.gameObject.name}. H");

                moveAmount.x = (hit.distance - skinWidth) * dirX;
                velocity.x = 0;
                if (dirX == -1) collisions.left = true;
                else collisions.right = true;
                break;
            }
        }

        transform.Translate(moveAmount);
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(Mathf.RoundToInt(bounds.size.y / dstBetweenRays), 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(Mathf.RoundToInt(bounds.size.x / dstBetweenRays), 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public bool IsGrounded() => collisions.below;
    public bool IsWallSliding() => (collisions.left || collisions.right) && !collisions.below;
    public bool IsTouchingLadder() => Physics2D.OverlapCircle(transform.position, 0.5f, Game.Layer.ladderLayer) != null;


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset() => above = below = left = right = false;
    }
}