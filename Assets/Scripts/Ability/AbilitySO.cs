using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Ability", menuName = "Special Abilities / Create Ability", order = 1)]
public class AbilitySO : ScriptableObject
{
    public enum Range { None,Self, Melee, Moderate, Long }

    public enum Type { InstantCast, DelayedCast, SetUp , SetUpAndInstantCast }

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
    public AttributeContainer abilityAttributie;

    [ShowIf("type", Type.SetUp | Type.SetUpAndInstantCast )]
    public AbilitySO[] connectedAbilities;
    
    public BuffSO buff;

    private Action<AbilitySO> OnUse = null;

    public Action<AbilitySO> OnAbilityEnd = null;

    private AbilityHolderUI abilityHolderUI;

    public T GetAbilityType<T>() where T : Enum
    {
        return (T)Enum.Parse(typeof(T), Utility.RemoveSpaceFromString(abilityName));
    }

    public void SetAbilityAction(Action<AbilitySO> useAction, Action<AbilitySO> endAction = null)
    {
        OnUse = useAction;
        OnAbilityEnd = endAction;
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

    public void EndAbility()
    {
        OnAbilityEnd?.Invoke(this);
    }

    public Action<AbilitySO> GetAbilityAction() => OnUse;

    public AbilityHolderUI GetAbilityHolderUI() => abilityHolderUI;
}

[Serializable]
public class AttributeContainer
{
    public Attribute[] attributes;

    public Attribute GetAbilityValueByID(string Key)
    {
        Attribute abilityValue = attributes.FirstOrDefault(x => x.key == Key);

        if (abilityValue != null) return abilityValue;
        else
        {
            Debug.LogError("Ability Value with ID: " + Key + " not found");
            return null;
        }
    }
}

[Serializable]
public class Attribute
{
    public enum Type { Float, Int, String, Float_Time}

    public enum TimeType { Seconds, Minutes }

    public Type valueType;

    [ShowIf("valueType", Type.Float_Time)]
    public TimeType timeType;
    
    public string key;

    [SerializeField] private string _value;

    public T GetValue<T>()
    {
        switch (valueType)
        {
            case Type.Int: return (T)(object)int.Parse(_value);
            case Type.Float: return (T)(object)float.Parse(_value);
            case Type.String: return (T)(object)_value;
            case Type.Float_Time: return (T)(object)((timeType == TimeType.Seconds) ? float.Parse(_value) : float.Parse(_value) * 60);
            default: return default;
        }
    }
}