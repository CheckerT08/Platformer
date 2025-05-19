using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Variables

    #region Movement Variables

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpingPower;
    [SerializeField] private float upwardsGravity;
    [SerializeField] private float downwardsGravity;
    [SerializeField] private float groundAccelerationTime;
    [SerializeField] private float airAccelerationTime;
    [SerializeField] private float maxFallSpeed;

    [Header("Ground Check")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask collidableLevelLayer;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Wall Jump")]
    [SerializeField] private float wallSlideMaxFallSpeed;
    [SerializeField] private Transform wallCheckTransform;
    [SerializeField] private Vector2 wallJumpPower;
    [SerializeField] private float wallJumpForcedTime;
    private bool isWallSliding;
    private float wallJumpDirectionLockTimer;
    private float wallJumpLockDirection;

    [Header("Ladder")]
    [SerializeField] private float ladderClimbSpeed;
    [SerializeField] private float ladderFallSpeed;
    [SerializeField] private LayerMask ladderLayer;
    
    [Header("Dash")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashJumpXVelocityMultiplier;
    private bool isDashing;
    private bool canDash;

    #endregion

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;
    private CameraFollowObject cameraFollowObject;
    private float fallVelYDampThresh;

    [Header("Misc")]
    [HideInInspector]public bool isFacingRight;
    private float horizontalInput;
    private float currentVelocity;
    private float timeSinceFallOffGround;
    private float vertical;
    private Rigidbody2D rb;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
    }

    private void Start()
    {
        fallVelYDampThresh = CameraManager.instance.fallSpeedDampingChangeThreshold;
    }

    private void Update()
    {
        TouchInput();
        Movement();
        CheckTurn();
        HandleCamera();
    }

    #endregion

    #region Input

    private void TouchInput()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        horizontalInput = 0f;
        foreach (Touch touch in Input.touches)
            CheckTouchZones(touch.position, touch.phase);
#endif
    }

    private void CheckTouchZones(Vector2 screenPos, UnityEngine.TouchPhase touchPhase = UnityEngine.TouchPhase.Began)
    {
        if (IsWithinUIArea(leftArea, screenPos)) horizontalInput = -1f;
        if (IsWithinUIArea(rightArea, screenPos)) horizontalInput = 1f;
        
        if (IsWithinUIArea(jumpArea, screenPos))
        {
            vertical = 1f;
            if (touchPhase == UnityEngine.TouchPhase.Began) Jump();
            if (touchPhase == UnityEngine.TouchPhase.Ended) CancelJump();
        }
        if (IsWithinUIArea(dashArea, screenPos))
        {
            if (touchPhase == UnityEngine.TouchPhase.Began) Dash();
        }
    }

    private bool IsWithinUIArea(RectTransform area, Vector2 screenPos)
    {
        return area != null && RectTransformUtility.RectangleContainsScreenPoint(area, screenPos);
    }

#if UNITY_EDITOR    
    public void InputMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump();
        if (context.canceled) CancelJump();
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (context.performed) Dash();
    }
#endif

    #endregion

    #region Movement

    private void Movement()
    {
        if (wallJumpDirectionLockTimer > 0f)
            wallJumpDirectionLockTimer -= Time.deltaTime;
        if (isDashing) return;
        rb.gravityScale = rb.velocity.y < 0f ? downwardsGravity : upwardsGravity;
        
        if (IsGround())
        {
            timeSinceFallOffGround = 0f;
            canDash = true;
        }
        else
        {
            timeSinceFallOffGround += Time.deltaTime;
        }
        
        float moveInput = wallJumpDirectionLockTimer > 0f ? wallJumpLockDirection : horizontalInput; // Wall Jump Lock

        Ladder();
        WallSlide();
        Velocity(moveInput);
    }

    private void WallSlide()
    {
        isWallSliding = !IsGround() && IsWall() && horizontalInput != 0f;
        if (isWallSliding)
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideMaxFallSpeed, float.MaxValue));
    }

    private void Velocity(float moveInput)
    {
        float targetVelocityX = IsWall() && !IsGround() ? 0f : moveInput * speed;
        float accelerationTime = IsGround() ? groundAccelerationTime : airAccelerationTime;

        float velocityX = Mathf.SmoothDamp(rb.velocity.x, targetVelocityX, ref currentVelocity, accelerationTime);
        float velocityY = Mathf.Clamp(rb.velocity.y, -maxFallSpeed, maxFallSpeed);

        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private bool Ladder()
    {
        if (wallJumpDirectionLockTimer > 0f) return false;
        if (!IsLadder()) return false;    
        rb.gravityScale = 0f;

        float velocityX = rb.velocity.x * 0.9f;
        float velocityY = vertical == 1f ? ladderClimbSpeed : ladderFallSpeed;
        rb.velocity = new Vector2(velocityX, velocityY);
    
        return true;
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
        float originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(GetDirection() * dashSpeed, 0f);
        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            elapsed += Time.deltaTime;
            if (IsWall()) break;
            yield return null;
        }
        rb.gravityScale = originalGravityScale;
        isDashing = false;
        rb.velocity = new Vector2(rb.velocity.x * 0.7f, rb.velocity.y);
    }

    private void Jump()
    {
        vertical = 1f;

        if (isWallSliding)
        {
            Turn();            
            rb.velocity = new Vector2(wallJumpPower.x * GetDirection(), wallJumpPower.y);

            // WallJump Lock
            wallJumpDirectionLockTimer = wallJumpForcedTime;
            wallJumpLockDirection = GetDirection();

            return;
        }

        if (isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x * dashJumpXVelocityMultiplier, jumpingPower);
            return;
        }

        if (timeSinceFallOffGround > coyoteTime) return;

        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        timeSinceFallOffGround = coyoteTime;
    }

    private void CancelJump()
    {
        if (rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.4f);
        vertical = 0f;
    }

    private bool IsWall()
    {
        if (IsLadder()) return false;
        return Physics2D.OverlapCircle(wallCheckTransform.position, 0.2f, collidableLevelLayer);
    }

    private bool IsGround()
    {
        Vector2 scanPosition = new Vector2(transform.position.x, transform.position.y - 1f);
        return Physics2D.OverlapBox(scanPosition, groundCheckSize, 0f, collidableLevelLayer);
    }

    private bool IsLadder()
    {
        if (IsGround()) return false;
        return Physics2D.OverlapCircle(transform.position - new Vector3(0f, 1f), 0.6f, ladderLayer);
    }

    private float GetDirection()
    {
        return transform.rotation.y == 0f ? 1f : -1f;
    }

    #endregion

    #region Orientation

    private void CheckTurn()
    {
        if (isDashing) return;
        if (wallJumpDirectionLockTimer > 0f) return; // Kein Drehen während Zwangsrichtung!
        if ((!isFacingRight && horizontalInput > 0f) || (isFacingRight && horizontalInput < 0f))
            Turn();
    }

    private void Turn()
    {
        float newYRotation = isFacingRight ? 180f : 0f;
        transform.rotation = Quaternion.Euler(0f, newYRotation, 0f);
        cameraFollowObject.CallTurn();
        isFacingRight = !isFacingRight;
    }

    #endregion

    #region Camera

    private void HandleCamera()
    {
        var cm = CameraManager.instance;

        if (rb.velocity.y < fallVelYDampThresh && !cm.isLerpingYDamping && !cm.lerpedFromPlayerFalling)
            cm.LerpYDamping(true);

        if (rb.velocity.y >= 0f && !cm.isLerpingYDamping && cm.lerpedFromPlayerFalling)
        {
            cm.lerpedFromPlayerFalling = false;
            cm.LerpYDamping(false);
        }
    }

    #endregion
}
