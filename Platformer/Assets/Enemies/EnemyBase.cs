using System.Collections;
using UnityEngine;

public class EnemyBase : Health
{
    [Header("Enemy Configuration")]
    public EnemyStats enemyStats;

    public float tickInterval = 0.2f;

    protected float speed => enemyStats.speed;
    protected float wallCheckDistance => enemyStats.wallCheckDistance;
    protected float groundCheckDistance => enemyStats.groundCheckDistance;
    protected LayerMask groundLayer => enemyStats.groundLayer;

    protected bool movingRight = true;

    private Coroutine tickCoroutine;

    public virtual void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("No EnemyStats assigned to " + gameObject.name);
            return;
        }

        float offset = Random.Range(0f, tickInterval);
        tickCoroutine = StartCoroutine(TickRoutine(offset));
    }

    private IEnumerator TickRoutine(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            OnTick();
            yield return new WaitForSeconds(tickInterval);
        }
    }

    protected virtual void OnTick() { }

    public override void TakeDamage(float amt)
    {
        base.TakeDamage(amt);
    }

    public override void Die()
    {
        base.Die();
        if (tickCoroutine != null) StopCoroutine(tickCoroutine);
    }

    protected bool CheckWall()
    {
        Vector2 origin = transform.position;
        Vector2 dir = movingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(origin, dir, wallCheckDistance, groundLayer);
    }

    protected bool CheckCliff()
    {
        Vector2 origin = transform.position;
        float direction = movingRight ? 1 : -1;
        Vector2 groundCheckPos = origin + new Vector2(direction * 0.5f, 0);
        return !Physics2D.Raycast(groundCheckPos, Vector2.down, groundCheckDistance, groundLayer);
    }
}
