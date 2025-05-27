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
        float input = 0f;
        bool jumpHeld = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            input = -1f;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            input = 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            player.OnJumpInputDown();

        jumpHeld = Keyboard.current.spaceKey.isPressed;

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
                jumpHeld = true;
                if (touch.phase == TouchPhase.Began)
                    player.OnJumpInputDown();
            }

            if (player.dashRect.Contains(pos) && touch.phase == TouchPhase.Began)
            {
                player.Dash();
            }
        }
#endif
        player.SetDirectionalInput(input);
        player.SetJumpHeld(jumpHeld);
    }
}
