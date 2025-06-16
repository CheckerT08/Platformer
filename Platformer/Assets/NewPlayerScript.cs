using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerInput))]
public class NewPlayerScript : MonoBehaviour
{
    public float speed = 8f;
    
    public float accelerationTimeGrounded = 0.1f;
    public float accelerationTimeAirborne = 0.2f;
    
    public float upGravity = 30f;
    public float downGravity = 50f;
    public float maxFallSpeed = 20f;
    public bool gravityActive = true;

    public float maxWallSlideSpeed = 3f;

    public float ladderClimbSpeed = 5f;

    Vector2 velocity;
    bool isClimbingLadder;
    bool facingRight = true;
    const float skinWidth = .015f;
    const float dstBetweenRays;
    int horizontalRayCount = 5, verticalRayCount = 5;
    float horizontalRaySpacing, verticalRaySpacing;
    float velocityXSmoothing;
    float inputX;
    BoxCollider2D boxCollider;
    PlayerInput input;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    // AWAKE && START
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisions = new CollisionInfo();
        CalculateRaySpacing();
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
    
    // UPDATE
    void Update()
    {
        UpdateRaycastOrigins();
        Input();
        PrecalculateMovement();
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

    void Input()
    {
        //inputX = input
    }

    void ApplyGravity()
    {
        if (IsLadder()) return;

        
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
    }

    void PrecalculateMovement()
    {
        float targetVelocityX = inputX * speed;
        float accelTime = collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);

        if (IsLadder()
        {
            // velocity
        }
        else
        {
            float gravity = velocity.y > 0 ? upGravity : downGravity;
            velocity.y -= gravity * Time.deltaTime;
        }

        // Velocity clampen
        velocity.y = Mathf.Max(velocity.y, (IsWall() && inputX != 0) ? -maxWallSlideSpeed : -maxFallSpeed);
    }

    // STRUCTS
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

    // HELPER
    public bool IsGround() => collisions.below;
    public bool IsWall() => (collisions.left && !facingRight) || (collisions.right && facingRight);
    public bool IsLadder() => Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer) != null;
}
