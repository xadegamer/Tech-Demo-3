using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityControllerBase : MonoBehaviour
{
    [SerializeField] protected AbilitySOSet[] abilitySOSets;

    protected PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        AssignAbilityActions();
    }

    protected void AssignAbilityActions()
    {
        foreach (AbilitySOSet abilitySOSet in abilitySOSets)
        {
            AssignSetAbilityActions(abilitySOSet);
        }
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
