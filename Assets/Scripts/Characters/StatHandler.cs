using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    public float currentHealth;
    public float currentMana;
    public float currentPhysicalDamageReduction;
    public float currentattackSpeed;
    public float currentbaseDamage;
    public float currentChanceToHit;
    public float currentChanceToCrit;
    public float currentCriticalDamageMultipier;
    public float currentHealingMultipier;

    [SerializeField] private BuffSO[] buffSOs;

    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();
    
    public void Start()
    {
        Buff newBuff = new Buff(buffSOs[0], ReduceHealth);
        activeBuffs.Add(newBuff);
    }


    public void ReduceHealth(float damage)
    {
        Debug.Log("Reducing health by " + damage);
    }
}

[Serializable]
public class Buff
{
    public BuffSO buffSO;
    public float currentDuration;
    public Action<float> OnBuffActive;
    public Action OnBuffEnd;

    public Buff(BuffSO buffSO, Action<float> buffEffect)
    {
        this.buffSO = buffSO;
        this.OnBuffActive = buffEffect;
        currentDuration = buffSO.maxDuration;
    }

    public float GetNormalisedDuration() => currentDuration / buffSO.maxDuration;

    public void ActivateBuff(Action buffEffect)
    {
        buffEffect?.Invoke();

        if (buffSO.buffType == BuffType.Temporary) ReduceBuffCurrentDuration();
    }

    IEnumerator ReduceBuffCurrentDuration()
    {
        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            yield return null;
        }

        currentDuration = 0;

        OnBuffEnd?.Invoke();
    }
}
