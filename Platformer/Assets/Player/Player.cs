using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    Controller2D controller;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce;
    [SerializeField] private float upGravity;
    [SerializeField] private float downGravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float groundAccelerationTime = .1f;
    [SerializeField] private float airAccelerationTime = .2f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpLockTime;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Ladder")]
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private float ladderClimbSpeed;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollowGO;

    private Vector3 velocity;
    private float velocityXSmoothing;
    private Vector2 directionalInput;
    private bool jumpBuffered;
    private bool isWallJumping;
    private float wallJumpTimer;
    private float coyoteTimer;
    private float gravity;
    private CameraFollowObject cameraFollowObject;

    private bool isFacingRight = true;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        UpdateTimers();
        HandleCamera();
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        jumpBuffered = true;
    }

    void UpdateTimers()
    {
        if (controller.collisions.below)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (isWallJumping) wallJumpTimer -= Time.deltaTime;
        if (wallJumpTimer <= 0f) isWallJumping = false;

        if (jumpBuffered)
        {
            jumpBuffered = false;
            TryJump();
        }
    }

    void TryJump()
    {
        if (IsOnLadder()) // Ladder Climb
        {
            velocity.y = directionalInput.y > 0 ? ladderClimbSpeed : -ladderClimbSpeed;
        }
        else if (controller.collisions.left || controller.collisions.right && !controller.collisions.below) // Wall Jump
        {
            isWallJumping = true;
            wallJumpTimer = wallJumpLockTime;
            velocity = new Vector2(-GetFacingDirection() * wallJumpForce.x, wallJumpForce.y);
            Flip();
        }
        else if (coyoteTimer > 0f) // Normal Jump
        {
            velocity.y = jumpForce;
            coyoteTimer = 0f;
        }
    }

    void HandleWallSliding()
    {
        int wallDirX = (controller.collisions.left) ? -1 : 1;
        bool wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -maxFallSpeed)
            {
                velocity.y = -maxFallSpeed;
            }
        }
    }

    void CalculateVelocity()
    {
        float accelerationTime = controller.collisions.below ? groundAccelerationTime : airAccelerationTime;
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        gravity = velocity.y > 0f ? upGravity : downGravity;

        if (!isWallJumping)
        {
            velocity.y -= gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
        }

        if (controller.collisions.below && velocity.y < 0f)
        {
            velocity.y = 0f;
        }
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

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        cameraFollowObject.CallTurn();
    }

    private int GetFacingDirection()
    {
        return isFacingRight ? 1 : -1;
    }

    private bool IsOnLadder()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer);
    }
}
