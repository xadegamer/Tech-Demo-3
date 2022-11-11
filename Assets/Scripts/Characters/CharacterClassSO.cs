using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Create Character", order = 1)]
public class CharacterClassSO : ScriptableObjectBase
{
    [FoldoutGroup("Info")]
    public int ID;
    [FoldoutGroup("Info")]
    public string characterName;
    [FoldoutGroup("Info")]
    public Sprite characterIcon;

    public float health;
    public float mana;
    public float physicalDamageReduction;
    public float attackSpeed;

    [FoldoutGroup("Damage Stats")]
    public float minbaseDamage;
    [FoldoutGroup("Damage Stats")]
    public float maxbaseDamage;

    public float chanceToHit;
    public float chanceToCrit;
    public float criticalDamageMultipier;
    public float healingMultipier;

    [FoldoutGroup("Spell Stats")]
    public float spellChancetoCrit;
    [FoldoutGroup("Spell Stats")]
    public float spellCriticalDamageMultipier;

    [FoldoutGroup("Mana Stats")]
    public ManaRegen manaRegenWhileCasting;
    [FoldoutGroup("Mana Stats")]
    public ManaRegen manaRegenWhileNotCasting;
    [FoldoutGroup("Mana Stats")]
    public ManaRegen manaRegenWhileNotInCombat;

    public float spellHealingMultipier { get => healingMultipier; }


    public override int GetID()
    {
        return ID;
    }
}

[System.Serializable]
public class ManaRegen
{
    public float amount;
    public float seconds;
}

public enum Class
{
    Warrior, Shaman, Rogue, Druid, Mage
}
