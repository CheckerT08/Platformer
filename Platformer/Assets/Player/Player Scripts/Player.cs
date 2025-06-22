using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementBody))]
public class Player : MonoBehaviour, InputGetter
{
    public MovementBody motor { private set; get; }
    PlayerAttack playerAttack;

    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;
    [SerializeField] private RectTransform attackArea;
    [SerializeField] private RectTransform rangedAttackArea;

    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect, attackRect, rangedAttackRect;

    [Header("Jump")]
    public Vector2 wallJumpForce;
    public float coyoteTime = 0.2f;
    private float coyoteTimer;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashTime = 0.2f;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer;

    [Header("Ladder")]
    public float climbSpeed = 5f;

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
        if (isDashing) return;
        UpdateVariablesAndInput();
        HandleSpecialMovement();
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
        #region dash
        if (dashPressed && canDash)
        {
            canDash = false;
            isDashing = true;
            dashTimer = dashTime;
            StartCoroutine(Dash());
            return;
        }
        #endregion

        #region jump and ladder
        if (jumpPressedAndHeld == false)
        {
            motor.gravityActive = true;
            if (motor.IsWallSliding())
            {
                motor.OverrideVelocity(new Vector2(motor.facingRight ? -1 : 1 * wallJumpForce.x, wallJumpForce.y));
                motor.Flip();
            }
            else if (coyoteTimer > 0)
            {
                motor.Jump();
            }
        }
        else if (jumpPressedAndHeld == true)
        {
            if (motor.GetVelocity().y > -5 && motor.IsTouchingLadder())
            {
                motor.gravityActive = false;
                motor.OverrideVelocityY(climbSpeed);
            }
        }
        else
        {
            motor.gravityActive = true;
        }
        #endregion

        #region wall sliding
        if (motor.IsWallSliding() && motor.GetVelocity().y < 0)
        {
            motor.OverrideVelocityY(Mathf.Max(motor.GetVelocity().y, -motor.data.maxWallSlideSpeed));
        }
        #endregion
    }

     IEnumerator Dash()
    {
        motor.gravityActive = false;
        while (dashTimer > 0)
        {
            motor.OverrideVelocity(new Vector2((motor.facingRight ? 1 : -1) * dashSpeed, 0));
            dashTimer -= Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        motor.gravityActive = true;
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
        return new Rect(corners[0], corners[2] - corners[0]);
    }

}