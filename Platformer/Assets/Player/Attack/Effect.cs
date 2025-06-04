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
    Fire, Poison, Bleed,
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
        
    }
}

// ─────────── Effekte ───────────

public class FireEffect : BaseEffect
{
    private void Start()
    {
    }

    public override void OnEffectEnd()
    {
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
