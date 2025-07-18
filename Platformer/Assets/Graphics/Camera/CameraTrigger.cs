using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraTrigger : MonoBehaviour
{
    public CamMode left, right;
    private BoxCollider2D coll;

    public Vector2 pos;

    private void OnValidate()
    {
        coll = GetComponent<BoxCollider2D>();
        Debug.Assert(coll, $"Collider auf {gameObject.name} is null");
        coll.isTrigger = true;
        coll.includeLayers = Game.Layer.player;
        coll.excludeLayers = ~Game.Layer.player;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Vector2 exit = (collision.transform.position - coll.bounds.center).normalized;
        Debug.Assert(CameraManager.instance, "CamManager.Instance is null");
        CameraManager.instance.SwapCamera(left, right, exit, pos);
    }
}
