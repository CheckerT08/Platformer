using System.Collections;
using UnityEngine;

public class EnemyBase : Health
{
    [Header("Enemy Configuration")]
    public EnemyStats enemyStats;

    public float tickInterval = 0.2f;

    private Coroutine tickCoroutine;

    public virtual void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogWarning("No EnemyStats assigned to " + gameObject.name);
            return;
        }

        // Startet die Tick-Coroutine mit zufälligem Offset
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
}
