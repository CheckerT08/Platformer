using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedDampingChangeThreshold = -15f;

    public bool isLerpingYDamping { get; private set; }
    public bool lerpedFromPlayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normalPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (!allVirtualCameras[i].enabled) return;

            currentCamera = allVirtualCameras[i];
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        normalPanAmount = framingTransposer.m_YDamping;
    }

    #region Lerp Y Damping

    public void LerpYDamping(bool playerIsFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(playerIsFalling));
    }

    private IEnumerator LerpYAction(bool playerIsFalling)
    {
        isLerpingYDamping = true;

        // Grab starting damping amount
        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        // Determine end damping amount
        endDampAmount = playerIsFalling ? fallPanAmount : normalPanAmount;

        if (playerIsFalling) lerpedFromPlayerFalling = true;

        //Lerp pan amount
        float elapsed = 0f;
        while (elapsed < fallYPanTime)
        {
            elapsed += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsed / fallYPanTime));
            framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        isLerpingYDamping = false;
    }

    #endregion

    public void SwapCamera(CinemachineVirtualCamera left, CinemachineVirtualCamera right, Vector2 exitDir, Vector2 pos)
    {
        print("SwapCam");
        // if the current camera is the camera on the left and our trigger exit direction was on the right
        if (currentCamera == left && exitDir.x > 0f)
        {
            // activate the new camera
            right.enabled = true;

            // deactivate the old camera
            left.enabled = false;

            // set the new camera as the current camera
            currentCamera = right;

            // update our composer variable
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        // if the current camera is the camera on the left and our trigger exit direction was on the right
        if (currentCamera == right && exitDir.x < 0f)
        {
            // activate the new camera
            left.enabled = true;

            // deactivate the old camera
            right.enabled = false;

            // set the new camera as the current camera
            currentCamera = left;

            // update our composer variable
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }


}