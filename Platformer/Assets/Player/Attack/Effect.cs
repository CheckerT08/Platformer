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
        { EffectEnum.Bleed, typeof(BleedEffect) },
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
        if (targetGameObject.GetComponent(componentType) != null) return;

        var component = targetGameObject.AddComponent(componentType);
        if (component is BaseEffect baseEffect)
        {
            baseEffect.Init(duration);
        }
    }
}

public enum EffectEnum
{
    Fire, Poison, Bleed,
    Speed, Slowness, Haste, AntiHaste,
    Weakness, Strength, AttackWeakness, AttackStrength
}

// ─────────── Gemeinsame Effekt-Basisklasse ───────────
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

    protected virtual void OnEffectEnd()
    {
        // Optional: Cleanup, Animation stoppen, etc.
    }
}

// ─────────── Effekte ───────────

public class FireEffect : BaseEffect
{
    protected override void OnEffectEnd()
    {
        Debug.Log("FireEffect ended!");
    }
}

public class PoisonEffect : BaseEffect
{
}

public class BleedEffect : BaseEffect
{
}

public class SpeedEffect : BaseEffect
{
}

public class SlownessEffect : BaseEffect
{
}

public class HasteEffect : BaseEffect
{
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
