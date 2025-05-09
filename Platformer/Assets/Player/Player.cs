// GitHub Comment
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Variables
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;

    [Header("Movement")] 
    [SerializeField] private float speed = 8;
    [SerializeField] private float jumpingPower = 10;
    [SerializeField] private float upwardsGravity = 2;
    [SerializeField] private float downwardsGravity = 3.3f;
    [SerializeField] private float groundAccelerationTime = 0.1f;
    [SerializeField] private float airAccelerationTime = 0.2f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Misc")]
    private float horizontalInput;
    private Rigidbody2D rb;
    public bool isFacingRight = true; // CameraFollowObject
    private float currentVelocity;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;
    private CameraFollowObject cameraFollowObject;
    private float fallVelYDampThresh; // fallSpeedYDampingChangeThreshold
    #endregion

    #region Game Loop

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

    #region Touch Input

    private void TouchInput()
    {
#if UNITY_EDITOR // Else UNITY_ANDOID is buggy
#elif UNITY_ANDROID              
        horizontalInput = 0;
        foreach (Touch touch in Input.touches)
        {
            CheckTouchZones(touch.position, touch.phase);
        }
#endif
    }

    private void CheckTouchZones(Vector2 screenPos, UnityEngine.TouchPhase touchPhase = UnityEngine.TouchPhase.Began)
    {
        if (IsWithinUIArea(leftArea, screenPos))
            horizontalInput = -1;
        if (IsWithinUIArea(rightArea, screenPos))
            horizontalInput = 1;
        if (IsWithinUIArea(jumpArea, screenPos) && touchPhase == UnityEngine.TouchPhase.Began)
            Jump();
        if (IsWithinUIArea(jumpArea, screenPos) && touchPhase == UnityEngine.TouchPhase.Ended)
            CancelJump();
    }

    private bool IsWithinUIArea(RectTransform area, Vector2 screenPos)
    {
        if (area == null) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(area, screenPos);
    }

    #endregion

    #region PlayerInput (Editor / Gamepad)

#if UNITY_EDITOR
    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Jump();
        if (context.canceled)
            CancelJump();
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
        // Gravity
        rb.gravityScale = rb.velocity.y < 0f ? downwardsGravity : upwardsGravity;

        // X Vel, Y Vel Clamp
        float targetVelocityX = horizontalInput * speed;
        float accelerationTime = IsGrounded() ? groundAccelerationTime : airAccelerationTime;
        float VelocityX = Mathf.SmoothDamp(rb.velocity.x, targetVelocityX, ref currentVelocity, accelerationTime);
        float VelocityY = Mathf.Clamp(rb.velocity.y, -maxFallSpeed, maxFallSpeed)
        rb.velocity = new Vector2(VelocityX, VelocityY);
    }

    private void Jump()
    {
        if (!IsGrounded()) return;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    private void CancelJump()
    {
        if (rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.4f);
    }    
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    #endregion

    #region Turn
    private void CheckTurn()
    {
        if (!isFacingRight && horizontalInput > 0f)
            Turn();
        else if (isFacingRight && horizontalInput < 0f)
            Turn();
    }

    private void Turn()
    {
        Vector3 rotation = new Vector3(transform.rotation.x, isFacingRight ? 180f : 0f, transform.rotation.z);
        transform.rotation = Quaternion.Euler(rotation);
        cameraFollowObject.CallTurn();
        isFacingRight = !isFacingRight;
    }

    #endregion

    #region Camera

    private void HandleCamera()
    {
        if (rb.velocity.y < fallVelYDampThresh && !CameraManager.instance.isLerpingYDamping && !CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (rb.velocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.lerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    #endregion
}
