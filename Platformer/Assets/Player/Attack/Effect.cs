using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

[System.Serializable]
public class Effect
{
    public EffectEnum effect;
    public float duration;

    // Dictionary von EffectEnum auf den zugehörigen MonoBehaviour-Typ
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
    
        // Vorhandene Komponente entfernen, falls vorhanden
        var existing = targetGameObject.GetComponent(componentType);
        if (existing != null)
            UnityEngine.Object.Destroy(existing);
    
        // Neue Komponente hinzufügen
        var component = targetGameObject.AddComponent(componentType);
    
        // Falls es eine BaseEffect ist, initialisieren
        if (component is BaseEffect baseEffect) // Wenn component von BaseEffect erbt
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

    public virtual void OnEffectEnd()
    {
        Destroy(this);
    }
}

// ─────────── Effekte ───────────

public class FireEffect : BaseEffect
{
    float time = 2 * Time.deltaTime; // Für genaue Anzahl an Ticks
    
    private void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            Tick();
            time = 0;
        }
    }

    void Tick()
    {
        Game.Damager.Damage(gameObject, 10);
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
    }
}

public class PoisonEffect : BaseEffect
{
    float time = 2 * Time.deltaTime; // Für genaue Anzahl an Ticks

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 0.5)
        {
            Tick();
            time = 0;
        }
    }

    void Tick()
    {
        Game.Damager.Damage(gameObject, 7);
    }
    

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
    }

}

public class SpeedEffect : BaseEffect
{
    CharacterMotor2D motor = GetComponent<CharacterMotor2D>();
    float original;
    
    void Start()
    {
        original = motor.speed;
        motor.speed *= 1.5f;
    }

    public override void OnEffectEnd()
    {
        motor.speed = original;
        base.OnEffectEnd();
    }
}

public class SlownessEffect : BaseEffect
{
    CharacterMotor2D motor = GetComponent<CharacterMotor2D>();
    float original;
    
    void Start()
    {
        original = motor.speed;
        motor.speed *= 0.5f;
    }

    public override void OnEffectEnd()
    {
        motor.speed = original;
        base.OnEffectEnd();
    }

}

public class HasteEffect : BaseEffect
{
    // Attacker Component schreiben
    Attacker attacker = GetComponentInChildren<Attacker>(); // Oder in selber
    float original
    
    void Start()
    {
        if (attacker == null) 
        {
            Destroy(this);
            return;
        }
        original = attacker.attackSpeed;
        attacker.attackSpeed *= 1.5f;
    }

    public override void OnEffectEnd()
    {
       attacker.attackSpeed = original;
        base.OnEffectEnd();
    }
}

public class AntiHasteEffect : BaseEffect
{
}

public class WeaknessEffect : BaseEffect
{
}

public class StrengthEffect : BaseEffect
{
}

public class AttackWeaknessEffect : BaseEffect
{
}

public class AttackStrengthEffect : BaseEffect
{
}
