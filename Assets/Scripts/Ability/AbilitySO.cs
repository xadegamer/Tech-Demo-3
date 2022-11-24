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

    public T GetAbilityType<T>() where T : Enum
    {
        return (T)Enum.Parse(typeof(T), Utility.RemoveSpaceFromString(abilityName));
    }
}

[Serializable]
public class Ability
{
    public AbilitySO abilitySO;
    public List<Ability> connectedAbilities = new List<Ability>();
    public Buff buff;

    private Action<Ability> OnUse;
    public Action<Ability> OnAbilityEnd;
    public AbilityHolderUI abilityHolderUI;

    public Ability(AbilitySO abilitySO)
    {
        this.abilitySO = abilitySO;
    }

    public void SetAbilityAction(Action<Ability> OnUse, Action<Ability> OnAbilityEnd = null)
    {
        this.OnUse = OnUse;
        this.OnAbilityEnd = OnAbilityEnd;
    }
    public float GetAbilityCost(float baseMana) => Utility.CalculatePercentageOfValue(baseMana, abilitySO.abilityCost);

    public void UseAbility()
    {
        OnUse?.Invoke(this);
    }

    public void EndAbility()
    {
        OnAbilityEnd?.Invoke(this);
    }
}
[Serializable]
public class AbilitySet
{
    public List<Ability> abilities;

    public AbilitySet(AbilitySetSO abilitySetSOs)
    {
        abilities = new List<Ability>();
        foreach (var abilitySO in abilitySetSOs.abilities)
        {
            Ability newAbility = new Ability(abilitySO);
            foreach (var connectedAbility in abilitySO.connectedAbilities) newAbility.connectedAbilities.Add(new Ability(connectedAbility));
            abilities.Add(newAbility);
        }
    }
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