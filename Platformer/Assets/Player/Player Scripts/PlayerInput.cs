using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.Touch;
using TouchPhase = UnityEngine.TouchPhase;

public class PlayerInputHandler : MonoBehaviour
{
    public Player player;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;

    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect;
    
    float input;
    bool jumpHeld;
    bool jumpPressed;
    bool dashPressed;

    void Awake()
    {
        var canvas = leftArea?.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            leftRect = GetScreenRect(leftArea);
            rightRect = GetScreenRect(rightArea);
            jumpRect = GetScreenRect(jumpArea);
            dashRect = GetScreenRect(dashArea);
        }

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
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 pos = touch.position;

            if (leftRect.Contains(pos))
                input = -1f;

            if (rightRect.Contains(pos))
                input = 1f;

            if (jumpRect.Contains(pos))
            {
                jumpHeld = true;
                if (touch.phase == TouchPhase.Began)
                    jumpPressed = true;
            }

            if (dashRect.Contains(pos) && touch.phase == TouchPhase.Began)
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

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

}
