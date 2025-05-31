using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerInputHandler : MonoBehaviour
{
    public Player player;

    float input;
    bool jumpHeld;
    bool jumpPressed;
    bool dashPressed;

    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        input = 0f;
        jumpHeld = false;
        jumpPressed = false;
        dashPressed = false;

#if UNITY_EDITOR
        // PC Input: Keyboard & Mouse
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // kein Keyboard gefunden

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            input = -1f;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            input = 1f;

        jumpHeld = keyboard.spaceKey.isPressed;
        jumpPressed = keyboard.spaceKey.wasPressedThisFrame;

        // Beispiel Dash mit Shift oder Maus links
        dashPressed = keyboard.leftShiftKey.wasPressedThisFrame;
#else
        foreach (Touch touch in Touch.activeTouches)
        {
            Vector2 pos = touch.screenPosition;

            if (player.leftRect.Contains(pos))
                input = -1f;

            if (player.rightRect.Contains(pos))
                input = 1f;

            if (player.jumpRect.Contains(pos))
            {
                jumpHeld = true;
                if (touch.phase == TouchPhase.Began)
                    jumpPressed = true;
            }

            if (player.dashRect.Contains(pos) && touch.phase == TouchPhase.Began)
                dashPressed = true;
        }
#endif

        if (jumpPressed)
            player.OnJumpInputDown();

        if (dashPressed)
            player.Dash();

        player.SetInput(input);
        player.SetJumpHeld(jumpHeld);
    }

    void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }
}
