using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityHandler : MonoBehaviour
{
    [SerializeField] private List<AbilitySO> selectedAbilities;
    
    private void Awake()
    {
        AssignAbilityActions();
    }

    private void Start()
    {

    }

    public void AssignAbilityActions()
    {
        foreach (AbilitySO abilitySO in selectedAbilities)
        {
            switch (abilitySO.GetAbilityType<PaladinAbilities>())
            {
                case PaladinAbilities.CrusaderStrike: abilitySO.SetAbilityAction(CrusaderStrike); break;
                case PaladinAbilities.HammerofJustice: abilitySO.SetAbilityAction(HammerofJustice);  break;
                case PaladinAbilities.DivineStorm: abilitySO.SetAbilityAction(DivineStorm); break;
                case PaladinAbilities.Judgement: abilitySO.SetAbilityAction(Judgement); break;
                default: break;
            }
        }
    }

    public void CrusaderStrike(AbilityData abilityData)
    {
        Debug.Log("Do Crusader Strike");
    }

    public void HammerofJustice(AbilityData abilityData)
    {
        
        Debug.Log("Do Hammer of Justice");
    }

    public void DivineStorm(AbilityData abilityData)
    {
        Debug.Log("Do Divine Storm");
    }

    public void Judgement(AbilityData abilityData)
    {
        Debug.Log("Do Judgement");
    }

    public void HolyLight(AbilityData abilityData)
    {
        Debug.Log("Do Holy Light");
    }

    public IEnumerator Wait(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }

    IEnumerator ChargeCast(Action OnStart, Action<float> IsPreparing,  Action OnEnd, float castTime)
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
