using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
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
        // Make the cameraFollowObject follow the player's position
        transform.position = playerTransform.position;
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