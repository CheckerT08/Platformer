using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Variables

    #region Movement Variables

    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 10f;
    [SerializeField] private float upwardsGravity = 2f;
    [SerializeField] private float downwardsGravity = 3.3f;
    [SerializeField] private float groundAccelerationTime = 0.1f;
    [SerializeField] private float airAccelerationTime = 0.2f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private LayerMask collidableLevelLayer;
    
    [Header("Ground Check")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Vector2 groundCheckSize = new(0.9f, 0.2f);

    [Header("Wall Jump")]
    [SerializeField] private float wallSlideMaxFallSpeed = 5f;
    [SerializeField] private Transform wallCheckTransform;
    [SerializeField] private Vector2 wallJumpPower = new(8f, 16f);
    private bool isWallSliding;

    [Header("Dash")]
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float dashSpeed = 10f;
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
    public bool isFacingRight = true;
    private float horizontalInput;
    private float currentVelocity;
    private float timeSinceFallOffGround;
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
        // For testing with Unity Editor
#elif UNITY_ANDROID
        horizontalInput = 0;
        foreach (Touch touch in Input.touches)
            CheckTouchZones(touch.position, touch.phase);
#endif
    }

    private void CheckTouchZones(Vector2 screenPos, UnityEngine.TouchPhase touchPhase = UnityEngine.TouchPhase.Began)
    {
        if (IsWithinUIArea(leftArea, screenPos)) horizontalInput = -1;
        if (IsWithinUIArea(rightArea, screenPos)) horizontalInput = 1;
        if (IsWithinUIArea(jumpArea, screenPos))
        {
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
        // DASH
        if (isDashing) return;

        // GRAVITY
        rb.gravityScale = rb.velocity.y < 0f ? downwardsGravity : upwardsGravity;

        // ON GROUND
        if (IsGrounded())
        {
            timeSinceFallOffGround = 0f;
            canDash = true;
        }
        else
        {
            timeSinceFallOffGround += Time.deltaTime;
        }


        // WALL JUMP
        WallSlide();

        // VELOCITY
        float targetVelocityX = horizontalInput * speed;
        float accelerationTime = IsGrounded() ? groundAccelerationTime : airAccelerationTime;

        float velocityX = Mathf.SmoothDamp(rb.velocity.x, targetVelocityX, ref currentVelocity, accelerationTime);
        float velocityY = Mathf.Clamp(rb.velocity.y, -maxFallSpeed, maxFallSpeed);

        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void WallSlide()
    {
        isWallSliding = !IsGrounded() && IsWalled() && horizontalInput != 0f;
        if (isWallSliding)
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideMaxFallSpeed, float.MaxValue));
    }

    private void WallJump()
    {
        Turn();
        rb.velocity = new Vector2(wallJumpPower.x * GetDirection(), wallJumpPower.y);
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
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravityScale;
        isDashing = false;
        rb.velocity = new Vector2(rb.velocity.x * 0.7f, rb.velocity.y);
    }

    private void Jump()
    {
        if (isWallSliding)
        {
            WallJump();
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
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheckTransform.position, 0.2f, collidableLevelLayer);
    }

    private bool IsGrounded()
    {

        Vector2 scanPosition = new Vector2(transform.position.x, transform.position.y - 1f);
        return Physics2D.OverlapBox(scanPosition, groundCheckSize, 0f, collidableLevelLayer);
    }

    private float GetDirection()
    {
        return transform.rotation.y == 0f ? 1f : -1f;
    }

    #endregion

    #region Orientation

    private void CheckTurn()
    {
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
