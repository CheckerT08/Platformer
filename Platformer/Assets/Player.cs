using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MovementBody))]
public class Player : MonoBehaviour, InputGetter
{
    MovementBody motor;
    PlayerAttack playerAttack;

    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;
    [SerializeField] private RectTransform attackArea;
    [SerializeField] private RectTransform rangedAttackArea;

    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect, attackRect, rangedAttackRect;



    [Header("Jump")]
    public float jumpForce = 15f;
    public Vector2 wallJumpForce;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    private float coyoteTimer;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashTime = 0.2f;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer;

    [Header("Ladder")]
    public LayerMask ladderLayer;
    public float climbSpeed = 5f;
    private bool isClimbingLadder;
    private Vector2 input;

    bool dashPressed;
    bool? jumpPressedAndHeld;

    void Awake()
    {
        motor = GetComponent<MovementBody>();
        playerAttack = GetComponent<PlayerAttack>();

        var canvas = leftArea?.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            leftRect = GetScreenRect(leftArea);
            rightRect = GetScreenRect(rightArea);
            jumpRect = GetScreenRect(jumpArea);
            dashRect = GetScreenRect(dashArea);
            attackRect = GetScreenRect(attackArea);
            rangedAttackRect = GetScreenRect(rangedAttackArea);
        }
    }

    void Update()
    {
        UpdateVariablesAndInput();
        HandleSpecialMovement();
        ApplyVelocityToMovementBody();
    }

    void UpdateVariablesAndInput()
    {
        if (motor.IsGrounded())
            canDash = true;
        coyoteTimer = motor.IsGrounded() ? coyoteTime : coyoteTimer - Time.deltaTime;
        dashPressed = GetDashPressed();
        jumpPressedAndHeld = GetJumpInputHeld();
    }

    void HandleSpecialMovement()
    {
        #region jump
        if (jumpPressedAndHeld == false)
        {
            if (motor.IsWallSliding())
            {
                motor.OverrideVelocity(new Vector2(dir * wallJumpForce.x, wallJumpForce.y));
                motor.Flip();
            }
            else if (coyoteTimer > 0)
            {
                motor.Jump(jumpForce);
            }
        }
        #endregion

        #region wall sliding
        if (motor.IsWallSliding() && motor.GetVelocity.y < 0)
        {
            motor.OverrideVelocity(new Vector2(motor.GetVelocity.x, Mathf.Max(motor.GetVelocity.y, -motor.data.maxWallSlideSpeed)))
        }
        #endregion
    }

    private void ApplyVelocityToMovementBody()
    {
        if (isDashing) return;

        if (isClimbingLadder)
        {
            motor.gravityActive = false;
            motor.velocity.y = input.y * climbSpeed;
        }
        else
        {
            motor.gravityActive = true;
        }
    }

    private void HandleDash()
    {
        if (!isDashing) return;

        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0)
        {
            isDashing = false;
        }
        else
        {
            motor.velocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, 0);
        }
    }

    private void HandleLadder()
    {
        isClimbingLadder = Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer) && input.y != 0 && jumpHeld;
    }

    public float GetXInput()
    {
        float input = 0;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            input = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            input = 1f;
#else
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 pos = touch.position;

            if (leftRect.Contains(pos))
                input = -1f;

            if (rightRect.Contains(pos))
                input = 1f;
        }
#endif
        return input;
    }

    bool? GetJumpInputHeld()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            return false;   // pressed this frame
        if (Input.GetKey(KeyCode.Space))
            return true;    // held
        return null;        // no input
#else
    if (Input.touchCount == 0)
        return null;

    for (int i = 0; i < Input.touchCount; i++)
    {
        Touch touch = Input.GetTouch(i);
        if (!jumpRect.Contains(touch.position))
            continue;

        if (touch.phase == TouchPhase.Began)
            return false;  // pressed this frame
        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            return true;   // held
    }
    return null;  // no relevant touch
#endif
    }

    bool GetDashPressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.LeftShift);
#else
    for (int i = 0; i < Input.touchCount; i++)
    {
        Touch touch = Input.GetTouch(i);
        if (dashRect.Contains(touch.position) && touch.phase == TouchPhase.Began)
            return true;
    }
    return false;
#endif
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        string cornerlog = string.Join(", ", corners);
        Game.Logger.Log("Getting Screen Rect " + rectTransform.gameObject.name + cornerlog);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

}