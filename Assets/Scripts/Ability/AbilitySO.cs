using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Ability", menuName = "Special Abilities / Create Ability", order = 1)]
public class AbilitySO : ScriptableObject
{
    public enum Range { Melee, Moderate, Long }

    public enum Type { InstantCast, DelayedCast, SetUp}

    [FoldoutGroup("Info")]
    public string abilityName;
    [FoldoutGroup("Info")]
    [TextArea(10, 10)]
    public string abilityDescription;
    [FoldoutGroup("Info")]
    public Sprite abilityIcon;

    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public float abilityCost;
    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public Type type;
    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public Range  range;
    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public AbilityData abilityData;

    [ShowIf("type", Type.SetUp)]
    public AbilitySO[] connectedAbilities;

    private Action<AbilitySO> OnUse = null;

    private AbilityHolderUI  abilityHolderUI;

    public T GetAbilityType<T>() where T : Enum
    {
        return (T)Enum.Parse(typeof(T), Utility.RemoveSpaceFromString(abilityName));
    }

    public void SetAbilityAction(Action<AbilitySO> action)
    {
        OnUse = action;
    }

    public void SetAbilityHolderUI(AbilityHolderUI abilityHolderUI)
    {
        this.abilityHolderUI = abilityHolderUI;
    }

    public float GetAbilityCost(float baseMana)=> Utility.CalculatePercentageOfValue(baseMana, abilityCost);

    public void UseAbility()
    {
        OnUse?.Invoke(this);
    }

    public Action<AbilitySO> GetAbilityAction()
    {
        return OnUse;
    }

    public AbilityHolderUI GetAbilityHolderUI()
    {
        return abilityHolderUI;
    }
}

[Serializable]
public class AbilitySOSet
{
    public AbilitySO[] abilities;
}

[Serializable]
public class AbilityData
{
    public AbilityValue[] values;

    public AbilityValue GetAbilityValueByID(string ID)
    {
        AbilityValue abilityValue = values.FirstOrDefault(x => x.ID == ID);

        if (abilityValue != null) return abilityValue;
        else
        {
            Debug.LogError("Ability Value with ID: " + ID + " not found");
            return null;
        }
    }
}

[Serializable]
public class AbilityValue
{
    public enum Type { Direct, Percentage, Time}

    public enum TimeType { Seconds, Minutes }

    public string ID;
    
    public Type valueType;

    [ShowIf("valueType", Type.Time)]
    public TimeType timeType;

    [SerializeField] private float value;

    public float GetValue()
    {
        switch (valueType)
        {
            case Type.Direct:
                return value;
            case Type.Percentage:
                return value;
            case Type.Time:
                return timeType ==  TimeType.Seconds ? value : value * 60;
            default: return 0;
        }
    }
}