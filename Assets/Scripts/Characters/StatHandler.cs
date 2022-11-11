using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerStatHandler : StatBase
{
    [SerializeField] private StatValue manaValue;

    [SerializeField] private float currentHealingMultipier;

    [SerializeField] private ManaRegen currentManaRegen;

    public override void SetUp(CharacterClassSO characterClassSO)
    {
        base.SetUp(characterClassSO);
        manaValue.SetValue(characterClassSO.mana, true);
        currentManaRegen = characterClassSO.manaRegenWhileNotInCombat;
        manaValue.SetRegenValue(currentManaRegen.regenInterval, currentManaRegen.regenAmount);
    }

    public StatValue GetManaValue()
    {
        return manaValue;
    }
}

[Serializable]
public class StatBase
{
    [field: SerializeField] public float currentPhysicalDamageReduction { get; private set; }
    [field: SerializeField] public float currentattackSpeed { get; private set; }
    [field: SerializeField] public float currentbaseDamage { get; private set; }
    [field: SerializeField] public float currentChanceToHit { get; private set; }
    [field: SerializeField] public float currentChanceToCrit { get; private set; }
    [field: SerializeField] public float currentCriticalDamageMultipier { get; private set; }

    [FoldoutGroup("Buff")]
    [SerializeField] private BuffSO[] allBuffsSO;
    [FoldoutGroup("Buff")]
    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();

    private CharacterClassSO characterClassSO;

    public virtual void SetUp(CharacterClassSO characterClassSO)
    {
        this.characterClassSO = characterClassSO;
        currentPhysicalDamageReduction = characterClassSO.physicalDamageReduction;
        currentattackSpeed = characterClassSO.attackSpeed;
        currentbaseDamage = characterClassSO.minbaseDamage;
        currentChanceToHit = characterClassSO.chanceToHit;
        currentChanceToCrit = characterClassSO.chanceToCrit;
        currentCriticalDamageMultipier = characterClassSO.criticalDamageMultipier;
    }

    public CharacterClassSO GetCharacterClassSO()
    {
        return characterClassSO;
    }

}

[Serializable]
public class StatValue
{
    public event EventHandler<float> OnValueChanged;

    [SerializeField] float maxValue;
    [SerializeField] float currentValue;
    
    [SerializeField] private float valueRegenInterval;
    [SerializeField] private float valueRegenAmount;

    private float valueRegenTimer;

    public void SetValue(float maxValue, bool fullAtStart)
    {
        this.maxValue = maxValue;
        currentValue = fullAtStart ? maxValue : 0;
        OnValueChanged?.Invoke(this, currentValue / this.maxValue);
    }

    public void SetRegenValue(float valueRegenInterval, float valueRegenAmount)
    {
        this.valueRegenInterval = valueRegenInterval;
        this.valueRegenAmount = valueRegenAmount;
        valueRegenTimer = valueRegenInterval;
    }

    public void ChangeRegenValue(float valueRegenInterval, float valueRegenAmount)
    {
        this.valueRegenInterval = valueRegenInterval;
        this.valueRegenAmount = valueRegenAmount;
    }

    public void ReduceValue(float amount)
    {
        currentValue = Mathf.Clamp(currentValue - amount, 0, maxValue);
        OnValueChanged?.Invoke(this, currentValue / maxValue);
    }

    public void IncreaseValue (float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        OnValueChanged?.Invoke(this, currentValue / maxValue);
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    public void ValueRegeneration()
    {
        if (currentValue < maxValue)
        {
            if (valueRegenTimer >= valueRegenInterval)
            {
                IncreaseValue(valueRegenAmount);
                valueRegenTimer = 0;
            }
            else valueRegenTimer += Time.deltaTime;
        }
    }
}