using UnityEngine;

[System.Serializable]
public class Effect
{
    public EffectEnum effect;
    public float duration;
}

public enum EffectEnum
{
    Fire, Poison, Bleed,
    Speed, Slowness, Haste, AntiHaste,
    Weakness, Strength, AttackWeakness, AttackStrength
}