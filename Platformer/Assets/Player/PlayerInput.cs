using UnityEngine;
using UnityEngine.InputSystem; // wichtig für das neue Input-System

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        Vector2 directionalInput = Vector2.zero;

        // Horizontal
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            directionalInput.x = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            directionalInput.x = 1;

        // Vertical
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            directionalInput.y = -1;
        else if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            directionalInput.y = 1;

        player.SetDirectionalInput(directionalInput);

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            player.OnJumpInputDown();
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            player.OnJumpInputUp();
        }
    }
}
