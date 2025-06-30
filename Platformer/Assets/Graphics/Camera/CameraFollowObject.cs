using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [HideInInspector] public static CamMode mode;
    [HideInInspector] public static Vector2 pos;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float flipVRotationTime = 0.5f;

    private Player player;

    private bool isFacingRight;

    private void Start()
    {
        enabled = true;
        player = playerTransform.gameObject.GetComponent<Player>();
        isFacingRight = player.motor.facingRight;
    }

    public void UpdateCamera()
    {
        Vector2 target = mode switch
        {
            CamMode.Horizontal => new Vector2(player.transform.position.x, pos.y),
            CamMode.Static => pos,
            _ => player.transform.position
        };

        transform.position = target;
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