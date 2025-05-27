using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    #region Variables

    private PlayerController controller;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float upGravity;
    [SerializeField] private float downGravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float groundAccelerationTime = 0.1f;
    [SerializeField] private float airAccelerationTime = 0.2f;

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

    [Header("Coyote Time & Buffer")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;

    private Vector2 velocity;
    private float velocityXSmoothing;
    private float input;

    private float jumpBufferCounter;
    private bool isJumpHeld;

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

    #endregion

    void Start()
    {
        controller = GetComponent<PlayerController>();
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

    void Update()
    {
        UpdateTimers();
        HandleInput();
        HandleLadderClimb();
        CalculateVelocity();
        HandleWallSliding();
        CheckTurn();
        Move();
        HandleCamera();
    }

    #region Input Handling

    void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            OnJumpInputDown();

        isJumpHeld = Keyboard.current.spaceKey.isPressed;
#endif
    }

    public void SetDirectionalInput(float newInput)
    {
        input = newInput;
    }

    public void OnJumpInputDown()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    public void SetJumpHeld(bool held)
    {
        isJumpHeld = held;
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    #endregion

    #region Movement

    void Move()
    {
        controller.Move(velocity * Time.deltaTime, input);

        if (controller.collisions.above || IsGround())
        {
            OnGround();
            velocity.y = 0;
            canDash = true;
        }
    }

    void CalculateVelocity()
    {
        if (isDashing) return;
        if (isWallJumping) input = GetDirection();

        float accelTime = IsGround() ? groundAccelerationTime : airAccelerationTime;
        float targetVelocityX = input * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);

        if (isClimbingLadder)
        {
            gravity = 0f;
            return;
        }

        gravity = velocity.y > 0f ? upGravity : downGravity;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
    }

    void OnGround()
    {
        if (velocity.y < 0f)
            velocity.y = 0f;
    }

    #endregion

    #region Jumping

    void UpdateTimers()
    {
        coyoteTimer = IsGround() ? coyoteTime : coyoteTimer - Time.deltaTime;

        if (isWallJumping) wallJumpTimer -= Time.deltaTime;
        if (wallJumpTimer <= 0f) isWallJumping = false;

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            TryJump();
        }
    }

    void TryJump()
    {
        if (IsWall() && coyoteTimer < 0f && input != 0f)
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpLockTime;
            velocity = new Vector2(-GetDirection() * wallJumpForce.x, wallJumpForce.y);
            Turn();
            jumpBufferCounter = 0f;
        }
        else if (coyoteTimer > 0f)
        {
            velocity.y = jumpForce;
            coyoteTimer = 0f;
            jumpBufferCounter = 0f;
        }
    }

    #endregion

    #region Wall Sliding

    void HandleWallSliding()
    {
        if (IsWall() && velocity.y < 0 && input != 0)
        {
            if (velocity.y < -maxWallSlideSpeed)
                velocity.y = -maxWallSlideSpeed;
        }
    }

    #endregion

    #region Dashing

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

    #endregion

    #region Ladder

    void HandleLadderClimb()
    {
        if (!IsLadder())
        {
            isClimbingLadder = false;
            return;
        }

        isClimbingLadder = isJumpHeld;
        velocity.y = isClimbingLadder ? ladderClimbSpeed : -ladderClimbSpeed;
    }

    #endregion

    #region Camera

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

    #endregion

    #region Direction

    void CheckTurn()
    {
        if (isDashing || wallJumpTimer > 0f) return;

        if ((!isFacingRight && input > 0f) || (isFacingRight && input < 0f))
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

    #endregion

    #region Utilities

    bool IsWall()
    {
        return (controller.collisions.left || controller.collisions.right) && !IsGround();
    }

    bool IsLadder()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer);
    }

    bool IsGround()
    {
        return controller.collisions.below;
    }

    #endregion
}
