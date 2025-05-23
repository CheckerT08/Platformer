using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    Controller2D controller;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce;
    [SerializeField] private float upGravity;
    [SerializeField] private float downGravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float groundAccelerationTime = .1f;
    [SerializeField] private float airAccelerationTime = .2f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpLockTime;
    [SerializeField] private float maxWallSlideSpeed;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Ladder")]
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private float ladderClimbSpeed;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;

    private Vector3 velocity;
    private float velocityXSmoothing;
    private Vector2 input;
    private bool jumpBuffered;
    private bool isWallJumping;
    private float wallJumpTimer;
    private float coyoteTimer;
    private float gravity;
    private CameraFollowObject cameraFollowObject;
    private bool isDashing;
    private bool canDash = true;
    private bool isClimbingLadder;
    public bool isFacingRight = true;
    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect;


    void Start()
    {
        controller = GetComponent<Controller2D>();
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();

#if UNITY_ANDROID
        var canvas = leftArea?.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            leftRect = GetScreenRect(leftArea);
            rightRect = GetScreenRect(rightArea);
            jumpRect = GetScreenRect(jumpArea);
            dashRect = GetScreenRect(dashArea);
        }
#endif
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    void Update()
    {
        UpdateTimers();
        HandleLadderClimb();
        CalculateVelocity();
        HandleWallSliding();
        CheckTurn();

        controller.Move(velocity * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
                canDash = true;
            }
        }

        HandleCamera();
    }

    public void SetDirectionalInput(Vector2 newInput)
    {
        input = newInput;
    }

    public void OnJumpInputDown()
    {
        jumpBuffered = true;
    }

    public void Dash()
    {
        if (!canDash || isDashing) return;
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = gravity;
        gravity = 0f;
        velocity = new Vector2(GetDirection() * dashSpeed, 0f);

        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            elapsed += Time.deltaTime;
            if (IsWall()) break;
            yield return null;
        }

        gravity = originalGravity;
        isDashing = false;
        velocity = new Vector2(velocity.x * 0.7f, velocity.y);
    }

    void UpdateTimers()
    {
        coyoteTimer = controller.collisions.below ? coyoteTime : coyoteTimer - Time.deltaTime;

        if (isWallJumping) wallJumpTimer -= Time.deltaTime;
        if (wallJumpTimer <= 0f) isWallJumping = false;

        if (jumpBuffered)
        {
            jumpBuffered = false;
            TryJump();
        }
    }

    void TryJump()
    {
        if (IsLadder())
        {
            isClimbingLadder = true;
            return;
        }

        if (IsWall() && coyoteTimer < 0f)
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpLockTime;
            velocity = new Vector2(-GetDirection() * wallJumpForce.x, wallJumpForce.y);
            Turn();
        }
        else if (coyoteTimer > 0f)
        {
            velocity.y = jumpForce;
            coyoteTimer = 0f;
        }
    }

    void HandleLadderClimb()
    {
        if (!IsLadder())
        {
            isClimbingLadder = false;
            return;
        }

        if (Keyboard.current.spaceKey.isPressed || jumpBuffered)
        {
            isClimbingLadder = true;
            velocity.y = ladderClimbSpeed;
        }
        else if (isClimbingLadder)
        {
            velocity.y = 0f;
        }
    }

    void CalculateVelocity()
    {
        if (isDashing) return;
        if (isWallJumping) input.x = GetDirection();

        float accelTime = controller.collisions.below ? groundAccelerationTime : airAccelerationTime;
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);

        if (isClimbingLadder)
        {
            gravity = 0f;
            return;
        }

        gravity = velocity.y > 0f ? upGravity : downGravity;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);

        if (controller.collisions.below && velocity.y < 0f)
        {
            velocity.y = 0f;
        }
    }

    void HandleWallSliding()
    {
        if ((controller.collisions.left || controller.collisions.right) &&
            !controller.collisions.below && velocity.y < 0 && input.x != 0)
        {

            if (velocity.y < -maxWallSlideSpeed)
                velocity.y = -maxWallSlideSpeed;
        }
    }

    void HandleCamera()
    {
        var cm = CameraManager.instance;

        if (velocity.y < cm.fallSpeedDampingChangeThreshold && !cm.isLerpingYDamping && !cm.lerpedFromPlayerFalling)
            cm.LerpYDamping(true);

        if (velocity.y >= 0f && !cm.isLerpingYDamping && cm.lerpedFromPlayerFalling)
        {
            cm.lerpedFromPlayerFalling = false;
            cm.LerpYDamping(false);
        }
    }

    void CheckTurn()
    {
        if (isDashing || wallJumpTimer > 0f) return;

        if ((!isFacingRight && input.x > 0f) || (isFacingRight && input.x < 0f))
            Turn();
    }

    void Turn()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = scale;

        cameraFollowObject.CallTurn();
    }

    int GetDirection()
    {
        return isFacingRight ? 1 : -1;
    }

    bool IsWall()
    {
        return (controller.collisions.left || controller.collisions.right) && !controller.collisions.below;
    }

    bool IsLadder()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer);
    }
}
