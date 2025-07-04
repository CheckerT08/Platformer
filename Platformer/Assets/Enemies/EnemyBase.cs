using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(MovementBody))]
public abstract class EnemyBase : MonoBehaviour, InputGetter
{
    [Header("Enemy Settings")]
    public EnemyStats enemyStats;
    public float tickInterval = 0.2f;
    protected bool movingRight = true;
    protected float speed;

    protected float wallCheckDistance;
    protected LayerMask groundLayer;
    protected MovementBody body;

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
        groundLayer = LayerMask.GetMask("Level Collidable");

        // Tick starten
        tickRoutine = StartCoroutine(TickLoop(Random.Range(0f, tickInterval)));

        body = GetComponent<MovementBody>();
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
    }

    public abstract float GetXInput();

    protected virtual void OnDeath()
    {
        if (tickRoutine != null) StopCoroutine(tickRoutine);
        Debug.Log($"{gameObject.name} is dead.");
    }
}
