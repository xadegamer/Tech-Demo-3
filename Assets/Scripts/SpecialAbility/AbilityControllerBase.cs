using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityControllerBase : MonoBehaviour
{
    [SerializeField] protected AbilitySOSet[] abilitySOSets;

    private void Awake()
    {
        AssignAbilityActions();
    }

    private void Start()
    {
        UIManager.Instance.SetAbilities(abilitySOSets);
    }

    protected void AssignAbilityActions()
    {
        AssignSetAbilityActions(abilitySOSets[0]);
        AssignSetAbilityActions(abilitySOSets[1]);
        AssignSetAbilityActions(abilitySOSets[2]);
        AssignSetAbilityActions(abilitySOSets[3]);
    }

    protected abstract void AssignSetAbilityActions(AbilitySOSet abilitySOSet);

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
