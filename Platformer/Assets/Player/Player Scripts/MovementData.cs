using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Data")]
public class MovementData : ScriptableObject
{
    public float speed = 8f;
    public float accelerationTimeGrounded = 0.1f;
    public float accelerationTimeAirborne = 0.2f;

    public float upGravity = 30f;
    public float downGravity = 50f;
    public float maxFallSpeed = 20f;
    public bool gravityActive = true;

    public float jumpForce = 20f;

    Vector2 wallJumpForce = new Vector2(10, 10);
    public float maxWallSlideSpeed = 3f;

    public float ladderClimbSpeed = 5f;

}
