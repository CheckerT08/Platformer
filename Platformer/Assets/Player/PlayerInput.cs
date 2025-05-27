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
        float input = 0;

#if UNITY_EDITOR || UNITY_STANDALONE

        // Tastatur
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            input = -1;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            input = 1;

        player.SetDirectionalInput(input);

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
                input = -1f;
            if (player.rightRect.Contains(pos))
                input = 1f;

            if (player.jumpRect.Contains(pos))
            {
                player.OnJumpInputDown();
            }

            if (player.dashRect.Contains(pos))
            {
                if (touch.phase == TouchPhase.Began) player.Dash();
            }
        }

        player.SetDirectionalInput(input);

#endif
    }
}
