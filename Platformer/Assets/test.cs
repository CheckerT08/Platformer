using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float gravity = 40f;
    [SerializeField] private float maxFallSpeed = 25f;
    [SerializeField] private float groundAccelerationTime = 0.05f;
    [SerializeField] private float airAccelerationTime = 0.2f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce = new Vector2(12f, 16f);
    [SerializeField] private float wallJumpLockTime = 0.2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;

    [Header("Ladder")]
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private float ladderClimbSpeed = 5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 1f);

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Touch Controls")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;

    private Vector2 velocity;
    private Vector2 input;
    private bool jumpBuffered;
    private bool dashBuffered;
    private bool isFacingRight = true;
    private bool isWallJumping;
    private bool isDashing;
    private float wallJumpTimer;
    private float coyoteTimer;
    private CameraFollowObject cameraFollowObject;

    private void Awake()
    {
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
    }

    private void Update()
    {
        HandleInput();
        UpdateTimers();
        ApplyPhysics();
        HandleCamera();
        CheckFlip();
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

    private bool IsWithinUIArea(RectTransform area, Vector2 screenPos)
    {
        return area != null && RectTransformUtility.RectangleContainsScreenPoint(area, screenPos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, 1f), groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(GetFacingDirection(), 0), wallCheckSize);
    }
}
