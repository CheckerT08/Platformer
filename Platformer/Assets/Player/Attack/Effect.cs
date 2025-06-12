using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Effect
{
    public EffectEnum effect;
    public float duration;

    private static readonly Dictionary<EffectEnum, Type> effectComponentMap = new()
    {
        { EffectEnum.Fire, typeof(FireEffect) },
        { EffectEnum.Poison, typeof(PoisonEffect) },
        { EffectEnum.Speed, typeof(SpeedEffect) },
        { EffectEnum.Slowness, typeof(SlownessEffect) },
        { EffectEnum.Haste, typeof(HasteEffect) },
        { EffectEnum.AntiHaste, typeof(AntiHasteEffect) },
        { EffectEnum.Weakness, typeof(WeaknessEffect) },
        { EffectEnum.Strength, typeof(StrengthEffect) },
        { EffectEnum.AttackWeakness, typeof(AttackWeaknessEffect) },
        { EffectEnum.AttackStrength, typeof(AttackStrengthEffect) }
    };

    public void ApplyTo(GameObject targetGameObject)
    {
        if (!effectComponentMap.TryGetValue(effect, out Type componentType)) return;

        var existing = targetGameObject.GetComponent(componentType);
        if (existing != null)
            UnityEngine.Object.Destroy(existing);

        var component = targetGameObject.AddComponent(componentType);

        if (component is BaseEffect baseEffect)
            baseEffect.Init(duration);
    }
}

public enum EffectEnum
{
    Fire, Poison,
    Speed, Slowness, Haste, AntiHaste,
    Weakness, Strength, AttackWeakness, AttackStrength
}

public abstract class BaseEffect : MonoBehaviour
{
    protected float duration;

    public virtual void Init(float duration)
    {
        this.duration = duration;
        StartCoroutine(RemoveAfterDuration());
    }

    private IEnumerator RemoveAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        OnEffectEnd();
        Destroy(this);
    }

    public virtual void OnEffectEnd() { }
}

// ─────────── Einzelne Effekte ───────────

public class FireEffect : TickEffect
{
    protected override float TickInterval => 1f;

    protected override void Tick()
    {
        Game.Damager.Damage(gameObject, 10);
    }
}

public class PoisonEffect : TickEffect
{
    protected override float TickInterval => 0.5f;

    protected override void Tick()
    {
        Game.Damager.Damage(gameObject, 7);
    }
}

public class SpeedEffect : ValueChangeEffect<CharacterMotor2D>
{
    void Start() => Init(
        GetComponent<CharacterMotor2D>(),
        m => m.speed,
        (m, v) => m.speed = v,
        1.5f
    );
}

public class SlownessEffect : ValueChangeEffect<CharacterMotor2D>
{
    void Start() => Init(
        GetComponent<CharacterMotor2D>(),
        m => m.speed,
        (m, v) => m.speed = v,
        0.5f
    );
}

public class HasteEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackSpeed,
        (a, v) => a.attackSpeed = v,
        1.5f
    );
}

public class AntiHasteEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackSpeed,
        (a, v) => a.attackSpeed = v,
        0.5f
    );
}

public class StrengthEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackDamageMultiplier,
        (a, v) => a.attackDamageMultiplier = v,
        1.5f
    );
}

public class WeaknessEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackDamageMultiplier,
        (a, v) => a.attackDamageMultiplier = v,
        0.5f
    );
}

public class AttackStrengthEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackPower,
        (a, v) => a.attackPower = v,
        1.5f
    );
}

public class AttackWeaknessEffect : ValueChangeEffect<Attacker>
{
    void Start() => Init(
        GetComponentInChildren<Attacker>(),
        a => a.attackPower,
        (a, v) => a.attackPower = v,
        0.5f
    );
}

// ─────────── Basisklassen ───────────

public abstract class TickEffect : BaseEffect
{
    protected abstract float TickInterval { get; }
    private float time;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= TickInterval)
        {
            Tick();
            time = 0;
        }
    }

    protected abstract void Tick();
}

public abstract class ValueChangeEffect<T> : BaseEffect where T : Component
{
    private T target;
    private Func<T, float> getter;
    private Action<T, float> setter;
    private float original;

    protected void Init(T target, Func<T, float> getter, Action<T, float> setter, float multiplier)
    {
        this.target = target;
        this.getter = getter;
        this.setter = setter;

        if (target == null)
        {
            Destroy(this);
            return;
        }

        original = getter(target);
        setter(target, original * multiplier);
    }

    public override void OnEffectEnd()
    {
        if (target != null)
            setter(target, original);

        base.OnEffectEnd();
    }
}

// DUMMY CLASS
public class Attacker : MonoBehaviour
{
    public float attackSpeed = 1f;
    public float attackDamageMultiplier = 1f;
    public float attackPower = 1f;
}
