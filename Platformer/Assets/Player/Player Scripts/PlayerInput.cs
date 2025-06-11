using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    Player player;
    PlayerAttack playerAttack;

    [Header("Touch Areas")]
    [SerializeField] private RectTransform leftArea;
    [SerializeField] private RectTransform rightArea;
    [SerializeField] private RectTransform jumpArea;
    [SerializeField] private RectTransform dashArea;
    [SerializeField] private RectTransform attackArea;
    [SerializeField] private RectTransform rangedAttackArea;

    [HideInInspector] public Rect leftRect, rightRect, jumpRect, dashRect, attackRect, rangedAttackRect;
    
    float inputX;
    bool jumpHeld;
    bool jumpPressed;
    bool dashPressed;

    void Start()
    {
        player = GetComponent<Player>();
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
        else 
        Game.Logger.Log("CANVAS IS NULL");
        Game.Logger.Log($"[Rects] left: {leftRect}, right: {rightRect}, jump: {jumpRect}, dash: {dashRect}, attack: {attackRect}, ranged: {rangedAttackRect}");
    }

    void Update()
    {
        inputX = 0f;
        jumpHeld = false;
        jumpPressed = false;
        dashPressed = false;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputX = 1f;

        jumpHeld = Input.GetKey(KeyCode.Space);
        jumpPressed = Input.GetKeyDown(KeyCode.Space);

        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
#else
    for (int i = 0; i < Input.touchCount; i++)
    {
        Touch touch = Input.GetTouch(i);
        Vector2 pos = touch.position;

        if (leftRect.Contains(pos))
            inputX = -1f;

        if (rightRect.Contains(pos))
            inputX = 1f;

        if (jumpRect.Contains(pos))
        {
            Game.Logger.Log("Jump Rect");
            jumpHeld = true;
            if (touch.phase == TouchPhase.Began)
                jumpPressed = true;
        }

        if (dashRect.Contains(pos) && touch.phase == TouchPhase.Began)
            dashPressed = true;

        if (attackRect.Contains(pos) && touch.phase == TouchPhase.Began)
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
        }
    }
#endif

        if (jumpPressed)
            player.OnJumpInputDown();

        if (dashPressed)
            player.Dash();

        player.SetInput(new Vector2(inputX, jumpHeld ? 1 : -1));
        player.SetJumpHeld(jumpHeld);
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
