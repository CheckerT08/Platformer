using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float groundAccelerationTime;
    [SerializeField] private float airAccelerationTime;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpLockTime;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Ladder")]
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private float ladderClimbSpeed;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 wallCheckSize;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;

    [Header("Touch Controls")]
    [SerializeField] private Canvas touchUICanvas;
    [SerializeField] private RectTransform leftArea; private Rect leftRect;
    [SerializeField] private RectTransform rightArea; private Rect rightRect;
    [SerializeField] private RectTransform jumpArea; private Rect jumpRect;
    [SerializeField] private RectTransform dashArea; private Rect dashRect;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;

    private Vector2 velocity;
    private Vector2 input;
    private bool jumpBuffered;
    private bool dashBuffered;
    private bool isFacingRight;
    private bool isWallJumping;
    private bool isDashing;
    private float wallJumpTimer;
    private float coyoteTimer;
    private CameraFollowObject cameraFollowObject;

    private void Awake()
    {
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
        leftRect = GetScreenRect(leftArea, canvas);
        rightRect = GetScreenRect(rightArea, canvas);
        jumpRect = GetScreenRect(jumpArea, canvas);
        dashRect = GetScreenRect(dashArea, canvas);
    }

    private void Update()
    {
        HandleInput();
        UpdateTimers();
        ApplyPhysics();
        CheckFlip();
        HandleCamera();
    }

    private void HandleInput()
    {
#if UNITY_EDITOR
        input.x = Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
        if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpBuffered = true;
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) dashBuffered = true;
#else
        input = Vector2.zero;
        foreach (Touch touch in Input.touches)
        {
            Vector2 pos = touch.position;
            if (IsWithinUIArea(leftArea, pos)) input.x = -1;
            if (IsWithinUIArea(rightArea, pos)) input.x = 1;
            if (IsWithinUIArea(jumpArea, pos) && touch.phase == TouchPhase.Began) jumpBuffered = true;
            if (IsWithinUIArea(dashArea, pos) && touch.phase == TouchPhase.Began) dashBuffered = true;
        }
#endif
    }

    private void UpdateTimers()
    {
        if (IsGrounded()) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (isWallJumping) wallJumpTimer -= Time.deltaTime;
        if (wallJumpTimer <= 0f) isWallJumping = false;
    }

    private void ApplyPhysics()
    {
        float accelerationTime = IsGrounded() ? groundAccelerationTime : airAccelerationTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x * speed, Time.deltaTime / accelerationTime);

        if (IsOnLadder())
        {
            velocity.y = input.y * ladderClimbSpeed;
        }
        else
        {
            if (!isDashing && !isWallJumping)
            {
                velocity.y -= gravity * Time.deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
            }
        }

        if (jumpBuffered)
        {
            jumpBuffered = false;
            TryJump();
        }

        if (dashBuffered)
        {
            dashBuffered = false;
            TryDash();
        }

        transform.Translate(velocity * Time.deltaTime);
    }

    private void TryJump()
    {
        if (IsWallTouching() && !IsGrounded())
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpLockTime;
            velocity = new Vector2(-GetFacingDirection() * wallJumpForce.x, wallJumpForce.y);
            Flip();
        }
        else if (coyoteTimer > 0f)
        {
            velocity.y = jumpForce;
            coyoteTimer = 0f;
        }
    }

    private void TryDash()
    {
        if (!isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        float originalGravity = gravity;
        gravity = 0f;
        velocity = new Vector2(GetFacingDirection() * dashSpeed, 0f);
        yield return new WaitForSeconds(dashTime);
        gravity = originalGravity;
        isDashing = false;
    }

    private void CheckFlip()
    {
        if (input.x > 0 && !isFacingRight || input.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        cameraFollowObject.CallTurn();
    }

    private int GetFacingDirection() => isFacingRight ? 1 : -1;

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(transform.position - new Vector3(0, 1f), groundCheckSize, 0f, groundLayer);
    }

    private bool IsWallTouching()
    {
        Vector3 offset = new Vector3(GetFacingDirection(), 0);
        return Physics2D.OverlapBox(transform.position + offset, wallCheckSize, 0f, groundLayer);
    }

    private bool IsOnLadder()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer);
    }

    private void HandleCamera()
    {
        var cm = CameraManager.instance;
        if (velocity.y < CameraManager.instance.fallSpeedDampingChangeThreshold && !cm.isLerpingYDamping && !cm.lerpedFromPlayerFalling)
            cm.LerpYDamping(true);

        if (velocity.y >= 0f && !cm.isLerpingYDamping && cm.lerpedFromPlayerFalling)
        {
            cm.lerpedFromPlayerFalling = false;
            cm.LerpYDamping(false);
        }
    }

    private bool IsWithinUIArea(Rect area, Vector2 screenPos)
    {
        return area.Contains(screenPos);
    }

    public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[2]);

        return new Rect(min, max - min);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, 1f), groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(GetFacingDirection(), 0), wallCheckSize);
    }
}
