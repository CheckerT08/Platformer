using TMPro;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    public CamMode mode
    {
        get => _mode;
        set
        {
            if (_mode != value)
            {
                _mode = value;
                followerDelayTimer = 0f;
            }
        }
    }
    private CamMode _mode;
    public Vector2 targetPosition { get; set; }
    private Vector2 followerPosition; 
    public float followerDelayTimer { get; set; } = 2f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float flipVRotationTime = 0.5f;

    private Player player;

    private bool isFacingRight;

    private void Start()
    {
        player = playerTransform.GetComponent<Player>();
        var startPos = player.transform.position;
        followerPosition = startPos;  // <-- Initialize here
        transform.position = startPos;
        enabled = true;
        isFacingRight = player.motor.facingRight;
    }

    public void Update()
    {
        followerDelayTimer += Time.deltaTime;
        Vector2 leader = mode switch
        {
            CamMode.Horizontal => new Vector2(player.transform.position.x, targetPosition.y),
            CamMode.Static => targetPosition,
            _ => (Vector2)player.transform.position
        };

        float dampStrength = followerDelayTimer < 2f ? mode switch
        {
            CamMode.Horizontal => 5f,
            CamMode.Static => 2f,
            _ => 4f - followerDelayTimer
        } : 1000f;

        float t = followerDelayTimer < 2f
            ? 1f - Mathf.Exp(-dampStrength * Time.deltaTime)
            : 1f;

        followerPosition = Vector2.Lerp(followerPosition, leader, t);

        Vector3 pos = transform.position;
        transform.position = new Vector3(followerPosition.x, followerPosition.y, pos.z);
        Debug.Log($"leader {leader} Target {targetPosition}, follower {followerPosition}, timer {followerDelayTimer}");

    }

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), flipVRotationTime).setEaseInOutSine();
    }

    private float DetermineEndRotation()
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }
}
