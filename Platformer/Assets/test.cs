// Vollständiger Raycast-basierter Player-Controller mit:
// Bewegung, Wall Jump, Dash, Leiter, Touch-Steuerung, Kamera

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float gravityUp = 1f;
    [SerializeField] private float gravityDown = 2f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing;
    private bool canDash;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce = new Vector2(12f, 16f);
    [SerializeField] private float wallJumpLockTime = 0.2f;

    [Header("Ladder")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask ladderLayer;

    [Header("Collision")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float skinWidth = 0.05f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float wallCheckDistance = 0.3f;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    private BoxCollider2D col;
    private Vector2 velocity;
    private bool isGrounded;
    private bool isTouchingWall;
    private float coyoteTimeCounter;
    private bool isFacingRight = true;
    private bool onLadder;
    private float horizontalInput;
    private float verticalInput;
    private float wallJumpLockCounter;

    private CameraFollowObject cameraFollow;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        cameraFollow = FindObjectOfType<CameraFollowObject>();
    }

    private void Update()
    {
        TouchInput();
        CheckCollisions();
        HandleCamera();
        ApplyGravity();
        HandleJump();
        HandleLadder();
        HandleDash();
        Move();
        HandleFlip();
    }

    private void TouchInput()
    {
#if UNITY_EDITOR
        // InputSystem Editor
        horizontalInput = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Jump();
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) Dash();
        verticalInput = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
#elif UNITY_ANDROID
        horizontalInput = 0f;
        verticalInput = 0f;
        foreach (Touch touch in Input.touches)
        {
            Vector2 pos = touch.position;
            if (IsInArea(leftArea, pos)) horizontalInput = -1f;
            if (IsInArea(rightArea, pos)) horizontalInput = 1f;
            if (IsInArea(jumpArea, pos) && touch.phase == TouchPhase.Began) Jump();
            if (IsInArea(dashArea, pos) && touch.phase == TouchPhase.Began) Dash();
            if (IsInArea(jumpArea, pos)) verticalInput = 1f;
        }
#endif
    }

    private bool IsInArea(RectTransform area, Vector2 screenPos)
    {
        return area != null && RectTransformUtility.RectangleContainsScreenPoint(area, screenPos);
    }

    private void CheckCollisions()
    {
        Vector2 origin = transform.position;
        isGrounded = Physics2D.BoxCast(origin, col.size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        isTouchingWall = Physics2D.Raycast(origin, Vector2.right * (isFacingRight ? 1 : -1), wallCheckDistance, groundLayer);
        onLadder = Physics2D.OverlapCircle(origin, 0.2f, ladderLayer);
    }

    private void ApplyGravity()
    {
        if (onLadder || isDashing) return;
        if (!isGrounded)
        {
            velocity.y += Physics2D.gravity.y * (velocity.y > 0 ? gravityUp : gravityDown) * Time.deltaTime;
        }
    }

    private void Move()
    {
        if (isDashing || onLadder) return;

        if (wallJumpLockCounter > 0)
        {
            wallJumpLockCounter -= Time.deltaTime;
            return;
        }

        velocity.x = horizontalInput * moveSpeed;
        transform.Translate(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canDash = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (onLadder)
        {
            velocity.y = jumpForce;
            return;
        }

        if (isTouchingWall && !isGrounded)
        {
            velocity = new Vector2(-GetFacingDirection() * wallJumpForce.x, wallJumpForce.y);
            wallJumpLockCounter = wallJumpLockTime;
            Flip();
            return;
        }

        if (coyoteTimeCounter > 0f)
        {
            velocity.y = jumpForce;
            coyoteTimeCounter = 0;
        }
    }

    private void Dash()
    {
        if (!canDash) return;
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        float time = 0;
        velocity = new Vector2(GetFacingDirection() * dashSpeed, 0);

        while (time < dashDuration)
        {
            transform.Translate(velocity * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private void HandleLadder()
    {
        if (!onLadder) return;
        velocity.y = verticalInput * climbSpeed;
        transform.Translate(new Vector2(0, velocity.y * Time.deltaTime));
    }

    private void HandleFlip()
    {
        if (horizontalInput > 0 && !isFacingRight)
            Flip();
        else if (horizontalInput < 0 && isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        cameraFollow?.CallTurn();
    }

    private float GetFacingDirection() => isFacingRight ? 1f : -1f;

    private void HandleCamera()
    {
        var cm = CameraManager.instance;
        if (velocity.y < cm.fallSpeedDampingChangeThreshold && !cm.isLerpingYDamping && !cm.lerpedFromPlayerFalling)
            cm.LerpYDamping(true);

        if (velocity.y >= 0 && !cm.isLerpingYDamping && cm.lerpedFromPlayerFalling)
        {
            cm.lerpedFromPlayerFalling = false;
            cm.LerpYDamping(false);
        }
    }
}
