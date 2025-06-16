// CharMotor.cs
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementBody : MonoBehaviour
{
    public MovementData data;

    float speed;
    float accelerationTimeGrounded;
    float accelerationTimeAirborne;
    float upGravity;
    float downGravity;
    float maxFallSpeed;
    bool gravityActive;
    float maxWallSlideSpeed;
    Vector2 wallJumpForce;
    float ladderClimbSpeed;
    float jumpForce;

    Vector2 vel;
    Vector2 input;
    BoxCollider2D coll;
    CollisionInfo collisions;
    RaycastOrigins raycastOrigins;

    const float skinWidth = .015f;
    const float dstBetweenRays = 0.2f;
    bool facingRight;
    int horizontalRayCount, verticalRayCount;
    float horizontalRaySpacing, verticalRaySpacing;
    float velocityXSmoothing;
    int dir;


    void Awake()
    {
        speed = data.speed;
        accelerationTimeGrounded = data.accelerationTimeGrounded;
        accelerationTimeAirborne = data.accelerationTimeAirborne;
        upGravity = data.upGravity;
        downGravity = data.downGravity;
        maxFallSpeed = data.maxFallSpeed;
        gravityActive = data.gravityActive;
        maxWallSlideSpeed = data.maxWallSlideSpeed;
        ladderClimbSpeed = data.ladderClimbSpeed;
        jumpForce = data.jumpForce;

        coll = GetComponent<BoxCollider2D>();
        collisions = new CollisionInfo();
        CalculateRaySpacing();
        dir = 1;
    }

    void Update()
    {
        UpdateRaycastOrigins();
        PrecalculateMovement();
    }

    #region RAYCASTS
    void CalculateRaySpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(Mathf.RoundToInt(bounds.size.y / dstBetweenRays), 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(Mathf.RoundToInt(bounds.size.x / dstBetweenRays), 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2); // Shrink

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    #endregion

    #region VELOCITY
    void PrecalculateMovement()
    {
        float targetVelocityX = input.x * speed;
        float accelerationTime = IsGround() ? accelerationTimeGrounded : accelerationTimeAirborne;
        vel.x = Mathf.SmoothDamp(vel.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        if (IsLadder() && input.y == 1f && vel.y > -5f)
        {
            vel.y = ladderClimbSpeed;
        }
        else
        {
            float gravity = vel.y > 0 ? upGravity : downGravity;
            vel.y -= gravity * Time.deltaTime;
        }

        // Velocity clampen
        vel.y = Mathf.Max(vel.y, (IsWall() && input.x != 0) ? -maxWallSlideSpeed : -maxFallSpeed);
    }
    #endregion

    #region PUBLIC VOIDS
    public void OverrideVelocity(Vector2 v)
    {
        vel = v;
    }

    public void SetInput(Vector2 i)
    {
        input = i;
        dir = (int)input.x;
    }

    public void Jump()
    {
        if (IsLadder()) return;
        if (IsGround()) ClassicJump();
        else if (IsWall()) WallJump();
    }
    #endregion

    #region JUMP
    void ClassicJump()
    {
        vel.y = jumpForce;
    }

    void WallJump()
    {
        // WALL JUMP VON CHARACTERMOTOR
        /*dir = -dir; 
        vel = new Vector2(dir * wallJumpForceX, wallJumpForceY);
        isFacingRight = dir > 0;*/
    }
    #endregion

    #region HELPER VOIDS
    public bool IsGround() => collisions.below;
    public bool IsWall() => (collisions.left && !facingRight) || (collisions.right && facingRight);
    public bool IsLadder() => Physics2D.OverlapCircle(transform.position, 0.5f, Game.Layer.ladderLayer) != null;
    #endregion

    #region STRUCTS
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset() => above = below = left = right = false;
    }
    #endregion
}
