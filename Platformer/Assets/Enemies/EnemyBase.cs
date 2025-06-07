using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyStats enemyStats;
    public float tickInterval = 0.2f;

    protected bool movingRight = true;
    protected float speed;

    protected float wallCheckDistance;
    protected float groundCheckDistance;
    protected LayerMask groundLayer;

    private Coroutine tickRoutine;

    protected virtual void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning($"No EnemyStats assigned to {gameObject.name}");
            return;
        }

        // Werte aus Stats übernehmen
        speed = enemyStats.speed;
        wallCheckDistance = enemyStats.wallCheckDistance;
        groundCheckDistance = enemyStats.groundCheckDistance;
        groundLayer = enemyStats.groundLayer;

        // Tick starten
        tickRoutine = StartCoroutine(TickLoop(Random.Range(0f, tickInterval)));
    }

    private IEnumerator TickLoop(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            OnTick();
            yield return new WaitForSeconds(tickInterval);
        }
    }

    protected virtual void OnTick()
    {
        Move();
        if (CheckWall() || CheckCliff()) Flip();
    }

    protected virtual void Move()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    protected void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected bool CheckWall()
    {
        Vector2 origin = transform.position;
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(origin, direction, wallCheckDistance, groundLayer);
    }

    protected bool CheckCliff()
    {
        Vector2 origin = transform.position;
        float direction = movingRight ? 1f : -1f;
        Vector2 checkPos = origin + new Vector2(direction * 0.5f, 0);
        return !Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
    }

    public virtual void OnDeath()
    {
        if (tickRoutine != null) StopCoroutine(tickRoutine);
        Debug.Log($"{gameObject.name} is dead.");
    }
}
