using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class GameUnitAbilityController : MonoBehaviour
{
    [SerializeField] protected AbilitySetSO[] abilitySetSOArray;
    
    [SerializeField] protected List<GameUnit> targets;

    [SerializeField] protected List<GameUnit> allies;

    protected GameUnit gameUnit;

    protected DamageInfo damageInfo;

    protected BuffManager buffManager;

    private void Awake()
    {
        gameUnit = GetComponent<GameUnit>();
        buffManager = GetComponent<BuffManager>();
        
        AssignAbilityActions();
        damageInfo = new DamageInfo();
    }

    protected void AssignAbilityActions()
    {
        abilitySetSOArray.ToList().ForEach(x => AssignSetAbilityActions(x));
    }

    protected abstract void AssignSetAbilityActions(AbilitySetSO abilitySetSO);

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
}
