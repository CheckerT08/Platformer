using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(MovementBody))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class EnemyBase : MonoBehaviour, InputGetter, Damageable
{
    [Header("Enemy Settings")]
    public EnemyStats enemyStatsPublic;
    private EnemyStats enemyStats;
    protected float tickInterval = 0.2f;
    protected bool movingRight = true;
    protected float wallCheckDistance;
    protected Health health;
    protected MovementBody body;
    protected BoxCollider2D coll;

    private Coroutine tickRoutine;

    protected virtual void Awake()
    {        
        health = GetComponent<Health>();
        body = GetComponent<MovementBody>();
        coll = GetComponent<BoxCollider2D>();

        if (enemyStats == null)
        {
            Debug.LogWarning($"No EnemyStats assigned to {gameObject.name}");
            return;
        }
        enemyStats = Instantiate(enemyStatsPublic);
        wallCheckDistance = coll.size.x;

        tickRoutine = StartCoroutine(TickLoop(Random.Range(0f, tickInterval)));

    }

    #region TICKING & ENEMY LOGIK
    private IEnumerator TickLoop(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            OnTick();
            yield return new WaitForSeconds(tickInterval);
        }
    }

    protected abstract void OnTick();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Game.Layer.Contains(Game.Layer.player, collision.gameObject.layer))
        {
            OnPlayerHit(collision);
        }
    }

    protected virtual void OnPlayerHit(Collider2D collision)
    {
        Game.Damager.Damage(collision.gameObject, enemyStats.damage);
    }

    public abstract float GetXInput();
    #endregion

    #region HEALTH

    public virtual void TakeDamage(float damageAmount)
    {
        health.ReduceHealth(damageAmount);
    }

    protected virtual void Die()
    {
        if (tickRoutine != null) StopCoroutine(tickRoutine);
    }
    #endregion
}