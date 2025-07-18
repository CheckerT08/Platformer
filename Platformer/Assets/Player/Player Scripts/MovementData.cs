using UnityEngine;

[CreateAssetMenu(menuName = "Movement Data")]
public class MovementData : ScriptableObject
{
    public float speed = 8f;
    public float accelerationTimeGrounded = 0.1f;
    public float accelerationTimeAirborne = 0.2f;
    [Space]
    public float upGravity = 30f;
    public float downGravity = 50f;
    public float maxFallSpeed = 20f;
    public bool gravityActive = true;
    [Space]
    public float jumpForce = 20f;
    [Space]
    Vector2 wallJumpForce = new(10, 10);
    public float maxWallSlideSpeed = 3f;
    [Space]
    public float ladderClimbSpeed = 5f;

}
