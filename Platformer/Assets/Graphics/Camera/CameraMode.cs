using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamMode
{
    Follow,
    Horizontal,
    Static
}

public class CameraMode : MonoBehaviour
{
    public CamMode mode;
}