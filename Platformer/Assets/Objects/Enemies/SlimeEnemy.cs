public class SlimeEnemy : EnemyBase
{
    [Header("Slime Specific Settings")]
    public Color slimeColor = Color.green;

    public float bounceSpeed = 1.5f;
    private float bounceTimer;

    protected override void Awake()
    {
        base.Awake();
        SetSlimeAppearance();
    }

    private void SetSlimeAppearance()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = slimeColor;
        }
    }

    private void Update()
    {
        Bounce();
    }

    private void Bounce()
    {
        bounceTimer += Time.deltaTime;
        float offset = Mathf.Sin(bounceTimer * bounceSpeed) * 0.05f;
        transform.localPosition = new Vector3(transform.localPosition.x, offset, transform.localPosition.z);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Slime explodes into goo!");
    }
}
