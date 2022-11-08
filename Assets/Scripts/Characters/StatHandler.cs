using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [SerializeField] private CharacterClassSO classAbilitySO;

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
    [SerializeField] private float manaRegenInterval;
    private float manaRegenTimer;
    private bool canRegenManner = true;

    [FoldoutGroup("Buff")]
    [SerializeField] private BuffSO[] buffSOs;
    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();

    [FoldoutGroup("SpecialAbility")]

    public void Start()
    {
        Buff newBuff = new Buff(buffSOs[0], ReduceHealth);
        activeBuffs.Add(newBuff);
    }

    private void Update()
    {
        ManaRegeneration();
    }

    public void ReduceHealth(float damage)
    {
        Debug.Log("Reducing health by " + damage);
    }

    public void ManaRegeneration()
    {
        if(canRegenManner && currentMana < classAbilitySO.mana)
        {
            if(manaRegenTimer >= manaRegenInterval)
            {
                currentMana += currentManaRegenAmount;
                manaRegenTimer = 0;
            }
            else  manaRegenTimer += Time.deltaTime;
        }
    }
}