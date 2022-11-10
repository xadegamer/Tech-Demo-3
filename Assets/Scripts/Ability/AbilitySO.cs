using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Special Abilities / Create Ability", order = 1)]
public class AbilitySO : ScriptableObject
{
    [FoldoutGroup("Info")]
    public string abilityName;
    [FoldoutGroup("Info")]
    [TextArea(10, 10)]
    public string abilityDescription;
    [FoldoutGroup("Info")]
    public Sprite abilityIcon;

    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public float abilityManaCost;
    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public AbilityData abilityData;

    private Action<AbilityData> OnUse = null;

    public T GetAbilityType<T>() where T : Enum
    {
        return (T)Enum.Parse(typeof(T), Utility.RemoveSpaceFromString(abilityName));
    }

    public void SetAbilityAction(Action<AbilityData> action)
    {
        OnUse = action;
    }

    public float GetManaCost(float baseMana)=> Utility.CalculateValueWithPercentage(baseMana, abilityManaCost, false);

    public void UseAbility()
    {
        OnUse?.Invoke(abilityData);
    }
}

[Serializable]
public class AbilityData
{
    public float [] values;
    public TimeDuration castTime;
    public TimeDuration coolDownTime;
}

[Serializable]
public class AbilitySOSet
{
    public AbilitySO[] abilities;
}

[Serializable]
public class TimeDuration
{
    public enum TimeType { Seconds,Minutes}
    public TimeType timeType;
    public float duration;

    public float GetTime()
    {
        return timeType == TimeType.Seconds ? duration : duration * 60;
    }
}