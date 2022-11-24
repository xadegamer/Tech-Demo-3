using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class GameUnitAbilityController : MonoBehaviour
{
    [SerializeField] protected AbilitySetSO[] abilitySetSOArray;
      
    [SerializeField] protected List<AbilitySet> abilities;

    [SerializeField] protected List<GameUnit> targets;

    [SerializeField] protected List<GameUnit> allies;

    protected GameUnit gameUnit;

    protected DamageInfo damageInfo;

    protected GameUnitBuffController buffManager;

    private void Awake()
    {
        gameUnit = GetComponent<GameUnit>();
        buffManager = GetComponent<GameUnitBuffController>();
        
        CreateAbility();

        damageInfo = new DamageInfo();
        damageInfo.owner = gameUnit;

        AssignAbilityActions();
    }

    public void CreateAbility()
    {
        foreach (AbilitySetSO abilitySetSO in abilitySetSOArray)
        {
            AbilitySet abilitySet = new AbilitySet(abilitySetSO);
            abilities.Add(abilitySet);
        }
    }

    protected void AssignAbilityActions()
    {
        abilities.ForEach(x => AssignSetAbilityActions(x));
    }

    protected abstract void AssignSetAbilityActions(AbilitySet abilitySetSO);


    protected IEnumerator Wait(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }

    protected IEnumerator ChargeCast(Action OnStart, Action<float> IsPreparing, Action OnEnd, float castTime)
    {
        OnStart?.Invoke();
        float activeTime = 0;
        while (activeTime < castTime)
        {
            activeTime += Time.deltaTime;
            IsPreparing?.Invoke(activeTime / castTime);
            yield return null;
        }
        OnEnd?.Invoke();
    }

    public AbilitySet GetAbilitySetSO(int index)
    {
        return abilities[index];
    }

    public Ability GetAbility(AbilitySO abilitySO)
    {
        return abilities.SelectMany(x => x.abilities).FirstOrDefault(x => x.abilitySO == abilitySO);
    }
}
