using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : AbilityControllerBase
{
    protected override void AssignSetAbilityActions(AbilitySOSet abilitySOSet)
    {
        foreach (AbilitySO abilitySO in abilitySOSet.abilities)
        {
            switch (abilitySO.GetAbilityType<PaladinAbilities>())
            {
                case PaladinAbilities.CrusaderStrike: abilitySO.SetAbilityAction(CrusaderStrike); break;
                case PaladinAbilities.HammerofJustice: abilitySO.SetAbilityAction(HammerofJustice); break;
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
}
