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
    
    #endregion

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;

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

    private void CheckTouchZones(Vector2 screenPos, TouchPhase touchPhase = TouchPhase.Began)
    {
        if (IsWithinUIArea(leftArea, screenPos)) horizontalInput = -1;
        if (IsWithinUIArea(rightArea, screenPos)) horizontalInput = 1;
        if (IsWithinUIArea(jumpArea, screenPos))
        {
            if (touchPhase == TouchPhase.Began) Jump();
            if (touchPhase == TouchPhase.Ended) CancelJump();
        }
    }

    private bool IsWithinUIArea(RectTransform area, Vector2 screenPos)
    {
        return area != null && RectTransformUtility.RectangleContainsScreenPoint(area, screenPos);
    }

#if UNITY_EDITOR
    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump();
        if (context.canceled) CancelJump();
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }
#endif

    #endregion

    #region Movement

    private void Movement()
    {
        rb.gravityScale = rb.velocity.y < 0f ? downwardsGravity : upwardsGravity;

        if (IsGrounded())
            timeSinceFallOffGround = 0f;
        else
            timeSinceFallOffGround += Time.deltaTime;

        WallSlide();

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
        float direction = transform.rotation.y == 0f ? 1f : -1f;
        rb.velocity = new Vector2(wallJumpPower.x * direction, wallJumpPower.y);
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
        return Physics2D.OverlapBox(groundCheckTransform.position, groundCheckSize, 0f, collidableLevelLayer);
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
