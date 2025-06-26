using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(InputGetter))]
public class MovementBody : MonoBehaviour
{
    public MovementData dataPublic;
    [HideInInspector] public MovementData data;

    [Header("Collision")]
    Vector2 velocity;
    [HideInInspector] public bool gravityActive;

    const float skinWidth = .015f;
    const float dstBetweenRays = .05f;
    int horizontalRayCount, verticalRayCount;
    float horizontalRaySpacing, verticalRaySpacing;

    float velocityXSmoothing;
    float inputX;
    bool movementBlocked = false;
    float blockTimer = 0f;
    bool ignoreInput = false;
    float ignoreInputTimer = 0f;
    public bool facingRight { get; private set; } = true;

    BoxCollider2D boxCollider;
    InputGetter inputGetter;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Awake()
    {
        data = Instantiate(dataPublic);
        boxCollider = GetComponent<BoxCollider2D>();
        inputGetter = GetComponent<InputGetter>();
        collisions = new CollisionInfo();
        CalculateRaySpacing();
    }

    void Update()
    {            
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0)
            movementBlocked = false;
        ignoreInputTimer -= Time.deltaTime;
        if (ignoreInputTimer <= 0)
            ignoreInput = false;

        UpdateRaycastOrigins();

        if (!movementBlocked)
        {
            if (ignoreInput) inputX = 0f;
            else GetInputFlip();
            ModifyVelocity();
        }

        Move(velocity * Time.deltaTime);
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

    void GetInputFlip()
    {
        inputX = inputGetter.GetXInput();
        if (inputX > 0 && !facingRight || inputX < 0 && facingRight)
            Flip();
    }

    void ModifyVelocity()
    {
        #region target x from input
        float targetVelocityX = inputX * data.speed;
        float accelTime = collisions.below ? data.accelerationTimeGrounded : data.accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);
        #endregion
        #region add gravity
        if (!gravityActive) return;
        float gravity = velocity.y > 0 ? data.upGravity : data.downGravity;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -data.maxFallSpeed, float.MaxValue);
        #endregion
    }

    public void BlockMovement(float time)
    {
        movementBlocked = true;
        blockTimer = time;
    }

    public void IgnoreInput(float time)
    {
        ignoreInput = true;
        ignoreInputTimer = time;
    }

    public void Jump()
    {
        if (collisions.below)
            velocity.y = data.jumpForce;
    }

    public void OverrideVelocity(Vector2 v)
    {
        velocity = v;
    }

    public void AddVelocity(Vector2 v)
    {
        velocity += v;
    }

    public void OverrideVelocityX(float v)
    {
        velocity.x = v;
    }

    public void OverrideVelocityY(float v)
    {
        velocity.y = v;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        OnFlip?.Invoke();
    }

    public event Action OnFlip;

    void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        float dirY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth + 0.05f;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = dirY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, Game.Layer.ground);
            if (hit)
            {
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
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, Game.Layer.ground);
            if (hit)
            {
                moveAmount.x = (hit.distance - skinWidth) * dirX;
                velocity.x = 0;
                if (dirX == -1) collisions.left = true;
                else collisions.right = true;
                break;
            }
        }

        transform.Translate(moveAmount);
        OnMove?.Invoke();
    }

    public event Action OnMove;

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
    public bool IsTouchingLadder() => Physics2D.OverlapCircle(transform.position, 0.5f, Game.Layer.ladder) != null;


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
