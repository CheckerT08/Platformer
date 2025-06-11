using UnityEngine;

[RequireComponent(typeof(CharacterMotor2D))]
public class Player : MonoBehaviour
{
    private CharacterMotor2D motor;

    [Header("Movement")]
    public bool isFacingRight = true;
    private float horizontalInput;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float wallJumpForceX = 12f;
    public float wallJumpForceY = 15f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    private float coyoteTimer;
    private float jumpBufferCounter;
    private bool jumpHeld;

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

    void Awake()
    {
        motor = GetComponent<CharacterMotor2D>();
    }

    void Update()
    {
        UpdateTimers();
        HandleInput();
        HandleJumpBuffer();
        HandleWallSlide();
        HandleDash();
        HandleLadder();
        ApplyMovement();
    }

    private void HandleInput()
    {
        // Input muss von außen per SetInput gesetzt werden
        horizontalInput = input.x;
        motor.SetInput(horizontalInput);

        if (horizontalInput > 0) isFacingRight = true;
        else if (horizontalInput < 0) isFacingRight = false;
    }

    private void ApplyMovement()
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

    private void HandleJumpBuffer()
    {
        coyoteTimer = motor.IsGrounded() ? coyoteTime : coyoteTimer - Time.deltaTime;

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (CanWallJump())
            {
                WallJump();
                jumpBufferCounter = 0;
            }
            else if (coyoteTimer > 0)
            {
                motor.ApplyJump(jumpForce);
                jumpBufferCounter = 0;
            }
        }
    }

    private bool CanWallJump()
    {
        return (motor.collisions.left || motor.collisions.right) && !motor.collisions.below;
    }

    private void WallJump()
    {
        float dir = motor.collisions.left ? 1f : -1f;
        motor.velocity = new Vector2(dir * wallJumpForceX, wallJumpForceY);
        isFacingRight = dir > 0;
    }

    private void HandleWallSlide()
    {
        if (CanWallJump() && motor.velocity.y < 0)
        {
            motor.velocity.y = Mathf.Max(motor.velocity.y, -motor.maxWallSlideSpeed);
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

    private void UpdateTimers()
    {
        if (!motor.IsGrounded() && !CanWallJump())
            canDash = true; // reset dash if airborne and not sliding
    }

    // --- Input API ---

    public void SetInput(Vector2 newInput)
    {
        input = newInput;
    }

    public void OnJumpInputDown()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    public void SetJumpHeld(bool held)
    {
        jumpHeld = held;
    }

    public void Dash()
    {
        if (!canDash || isDashing) return;

        isDashing = true;
        dashTimer = dashTime;
        canDash = false;
    }
}
