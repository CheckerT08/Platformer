using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera left, right;
    public BoxCollider2D coll;

    public Vector2 pos;

    private void OnTriggerExit2D(Collider2D collision)
    {
        Vector2 exit = (collision.transform.position - coll.bounds.center).normalized;
        // Y Übergeben wenn horizontal, pos übergeben wenn vertical
        CameraManager.instance.SwapCamera(left, right, exit);
    }
}
