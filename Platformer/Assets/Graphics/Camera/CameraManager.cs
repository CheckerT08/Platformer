using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CamMode currentMode;
    [SerializeField] private CameraFollowObject followObject;

    public bool isLerpingYDamping { get; private set; }
    public bool lerpedFromPlayerFalling { get; set; }

    private CinemachineFramingTransposer framingTransposer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SwapCamera(CamMode left, CamMode right, Vector2 exitDir, Vector2 pos)
    {
        if (currentMode == left && exitDir.x > 0f)
        {
            followObject.pos = pos;
            currentMode = followObject.mode = right;
            return;
        }

        if (currentMode == right && exitDir.x < 0f)
        {
            followObject.pos = pos;
            currentMode = followObject.mode = left;
            return;
        }
    }
}