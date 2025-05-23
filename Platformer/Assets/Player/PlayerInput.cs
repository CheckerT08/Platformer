using UnityEngine;
#if UNITY_EDITOR 
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        Vector2 directionalInput = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE

        // Tastatur
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            directionalInput.x = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            directionalInput.x = 1;

        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            directionalInput.y = -1;
        else if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            directionalInput.y = 1;

        player.SetDirectionalInput(directionalInput);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            player.OnJumpInputDown();
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            player.Dash();
        }

#elif UNITY_ANDROID

        foreach (Touch touch in Input.touches)
        {
            Vector2 pos = touch.position;

            if (player.leftRect.Contains(pos))
                directionalInput.x = -1f;
            if (player.rightRect.Contains(pos))
                directionalInput.x = 1f;

            if (player.jumpRect.Contains(pos))
            {
                directionalInput.y = 1f;
                if (touch.phase == TouchPhase.Began) player.OnJumpInputDown();
            }

            if (player.dashRect.Contains(pos))
            {
                if (touch.phase == TouchPhase.Began) player.Dash();
            }
        }

        player.SetDirectionalInput(directionalInput);

#endif
    }
}
