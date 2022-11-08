using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Create Ability", order = 1)]
public class ClassAbilitySO : ScriptableObjectBase
{
    [FoldoutGroup("Info")]
    public string abilityName;
    [FoldoutGroup("Info")]
    public string abilityDescription;
    [FoldoutGroup("Info")]
    public Sprite abilityIcon;

    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public float abilityValue;
    [FoldoutGroup("Stats")]
    [SuffixLabel("Sec")]
    public float abilityCooldown;
    [FoldoutGroup("Stats")]
    [SuffixLabel("%")]
    public float abilityManaCost;

    public float GetManaCost(float baseMana)=> Utility.CalculateValueWithPercentage(baseMana, abilityManaCost, false);

    public override int GetID()
    {
        throw new System.NotImplementedException();
    }
}
