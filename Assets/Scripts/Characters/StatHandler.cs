using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatHandler
{ 
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentMana;
    [SerializeField] private float currentPhysicalDamageReduction;
    [SerializeField] private float currentattackSpeed;
    [SerializeField] private float currentbaseDamage;
    [SerializeField] private float currentChanceToHit;
    [SerializeField] private float currentChanceToCrit;
    [SerializeField] private float currentCriticalDamageMultipier;
    [SerializeField] private float currentHealingMultipier;

    [FoldoutGroup("ManaRegen")]
    [SerializeField] private float currentManaRegenAmount;

    [FoldoutGroup("ManaRegen")]
    [SerializeField] private float manaRegenInterval;
    private float manaRegenTimer;
    private bool canRegenManner = true;

    [FoldoutGroup("Buff")]
    [SerializeField] private BuffSO[] buffSOs;
    [FoldoutGroup("Buff")]
    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();

    private CharacterClassSO characterClassSO;

    public void SetUp(CharacterClassSO characterClassSO)
    {
        this.characterClassSO = characterClassSO;
        Buff newBuff = new Buff(buffSOs[0], ReduceHealth);
        activeBuffs.Add(newBuff);
    }

    public void Update()
    {
        ManaRegeneration();
    }

    public void ReduceHealth(float damage)
    {
        Debug.Log("Reducing health by " + damage);
    }

    public void ManaRegeneration()
    {
        if(canRegenManner && currentMana < characterClassSO.mana)
        {
            if(manaRegenTimer >= manaRegenInterval)
            {
                currentMana += currentManaRegenAmount;
                manaRegenTimer = 0;
            }
            else  manaRegenTimer += Time.deltaTime;
        }
    }

    public CharacterClassSO GetCharacterClassSO()
    {
        return characterClassSO;
    }
}