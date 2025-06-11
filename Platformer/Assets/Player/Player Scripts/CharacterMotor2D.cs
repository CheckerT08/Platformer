using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMotor2D : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 6f;

    [Header("Gravity")]
    public float upGravity = 30f;
    public float downGravity = 50f;
    public float maxFallSpeed = 20f;
    public bool gravityActive = true;

    [Header("Acceleration")]
    public float accelerationTimeGrounded = 0.1f;
    public float accelerationTimeAirborne = 0.2f;

    [Header("Wall Slide")]
    public float maxWallSlideSpeed = 3f;

    [Header("Ladder")]
    public LayerMask ladderLayer;
    public float ladderClimbSpeed = 5f;

    // State
    public Vector2 velocity;
    private float velocityXSmoothing;
    private float inputX;
    public bool isClimbingLadder;

    // Components
    private BoxCollider2D boxCollider;

    public CollisionInfo collisions;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisions = new CollisionInfo();
    }

    void Update()
    {
        UpdateCollisions();
        ApplyGravity();

        float targetVelocityX = inputX * moveSpeed;
        float accelTime = collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);

        if (isClimbingLadder)
        {
            gravityActive = false;
            // velocity.y sollte vom Player kommen
        }
        else
        {
            gravityActive = true;
        }

        HandleWallSlide();

        Move(velocity * Time.deltaTime);
    }

    public void SetInput(float horizontalInput)
    {
        inputX = Mathf.Clamp(horizontalInput, -1f, 1f);
    }

    public void ApplyJump(float jumpForce)
    {
        if (collisions.below)
            velocity.y = jumpForce;
    }

    void ApplyGravity()
    {
        if (!gravityActive) return;

        float gravity = velocity.y > 0 ? upGravity : downGravity;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
    }

    void HandleWallSlide()
    {
        if (IsWall() && velocity.y < 0 && inputX != 0)
        {
            velocity.y = Mathf.Max(velocity.y, -maxWallSlideSpeed);
        }
    }

    void Move(Vector2 moveAmount)
    {
        Vector2 finalMove = moveAmount;

        // Boden-Kollision (rudimentär)
        if (collisions.below && finalMove.y < 0)
        {
            finalMove.y = 0;
            velocity.y = 0;
        }

        transform.Translate(finalMove);
    }

    void UpdateCollisions()
    {
        collisions.Reset();

        // Beispiel: rudimentäre Kollisionen (ersetzen durch Raycast in der finalen Version)
        collisions.below = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        collisions.left = Physics2D.Raycast(transform.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        collisions.right = Physics2D.Raycast(transform.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
    }

    public bool IsGrounded() => collisions.below;

    public bool IsWall() => (collisions.left || collisions.right) && !collisions.below;

    public bool IsTouchingLadder()
    {
        Collider2D ladderCheck = Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer);
        return ladderCheck != null;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = left = right = false;
        }
    }
}
