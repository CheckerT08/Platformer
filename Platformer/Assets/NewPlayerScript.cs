// player.cs
using UnityEngine;

[RequireComponent(typeof(MovementBody))]
[RequireComponent(typeof(PlayerAttack))]
public class NewPlayerScript : MonoBehaviour
{
    MovementBody body;
    PlayerAttack playerAttack;

    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;
    [SerializeField] private RectTransform attackArea;
    [SerializeField] private RectTransform rangedAttackArea;

    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect, attackRect, rangedAttackRect;

    Vector2 input;
    bool jumpPressed;
    bool dashPressed;


    private void Awake()
    {
        body = GetComponent<MovementBody>();
        playerAttack = GetComponent<PlayerAttack>();

        var canvas = leftArea?.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            leftRect = GetScreenRect(leftArea);
            rightRect = GetScreenRect(rightArea);
            jumpRect = GetScreenRect(jumpArea);
            dashRect = GetScreenRect(dashArea);
            attackRect = GetScreenRect(attackArea);
            rangedAttackRect = GetScreenRect(rangedAttackArea);
        }
    }

    void Update()
    {
        input = Vector2.zero;
        jumpPressed = false;
        dashPressed = false;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            input.x = 1f;

        if (Input.GetKey(KeyCode.Space))
            input.y = 1f;
        jumpPressed = Input.GetKeyDown(KeyCode.Space);

        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
#else
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 pos = touch.position;

            if (leftRect.Contains(pos))
                input.x = -1f;
            if (rightRect.Contains(pos))
                input.x = 1f;

            if (jumpRect.Contains(pos))
            {
                input.y = 1f;
                if (touch.phase == TouchPhase.Began)
                    jumpPressed = true;
            }

            if (dashRect.Contains(pos) && touch.phase == TouchPhase.Began)
                dashPressed = true;

            /*if (attackRect.Contains(pos) && touch.phase == TouchPhase.Began)
            {
                int idx = playerAttack.primaryAttackID;
                if (idx >= 0 && idx < playerAttack.attacks.Length)
                    playerAttack.TryAttack(playerAttack.attacks[idx]);
            }

            if (rangedAttackRect.Contains(pos) && touch.phase == TouchPhase.Began)
            {
                int idx = playerAttack.rangedAttackID;
                if (idx >= 0 && idx < playerAttack.attacks.Length)
                    playerAttack.TryAttack(playerAttack.attacks[idx]);
            }*/
        }
#endif
        body.SetInput(input);
        if (jumpPressed)
            body.Jump();

        // JUMP METHODEN AUF BODY
        /*if (jumpPressed)
            player.OnJumpInputDown();

        if (dashPressed)
            player.Dash();

        player.SetInput(new Vector2(inputX, jumpHeld ? 1 : -1));
        player.SetJumpHeld(jumpHeld);*/
    }


    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        string cornerlog = string.Join(", ", corners);
        Game.Logger.Log("Getting Screen Rect " + rectTransform.gameObject.name + cornerlog);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

}
