using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : AbilityControllerBase
{
    [SerializeField] AbilitySO currentJudgement;

    protected void Start()
    {
        UIManager.Instance.SetAbilities(abilitySOSets);
    }

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
                case PaladinAbilities.JudgementOfRighteousness: abilitySO.SetAbilityAction(JudgementOfRighteousness); break;
                case PaladinAbilities.JudgementofWisdom: abilitySO.SetAbilityAction(JudgementOfWisdom); break;
                case PaladinAbilities.JudgementofWeakness: abilitySO.SetAbilityAction(JudgementOfWeakness); break;
                default: break;
            }
        }
    }

    public void CrusaderStrike(AbilitySO  abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(playerManager.StatHandler.GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);
        Debug.Log("Do Crusader Strike : " + damage);
    }

    public void HammerofJustice(AbilitySO abilitySO)
    {
        float stunDuration = abilitySO.abilityData.GetAbilityValueByID("StunDuration").GetValue();
        Debug.Log("Do Hammer of Justice : " + stunDuration);
    }

    public void DivineStorm(AbilitySO abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(playerManager.StatHandler.GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);
        float healAmount = Utility.CalculatePercentageOfValue(damage, abilitySO.abilityData.GetAbilityValueByID("Heal").GetValue());
        Debug.Log("Do Divine Storm : " + damage);
        Debug.Log("Heal : " + healAmount);
    }

    public void Judgement(AbilitySO abilitySO)
    {
        float damage = Utility.CalculatePercentageOfValue(playerManager.StatHandler.GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BaseWeaponDamage").GetValue());
        Debug.Log("Do Judgement : " + damage);

        switch (abilitySO.GetAbilityType<PaladinAbilities>())
        {
            case PaladinAbilities.JudgementOfRighteousness:

                Debug.Log("Do Judgement of Righteousness : " + damage);

                break;
            case PaladinAbilities.JudgementofWisdom:

                Debug.Log("Do Judgement of Wisdom : " + damage);

                break;
            case PaladinAbilities.JudgementofWeakness:

                Debug.Log("Do Judgement of Weakness : " + damage);

                break;
            default: break;
        }
    }

    public void JudgementOfRighteousness(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Righteousness");
    }

    public void JudgementOfWisdom(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Wisdom");
    }
    
    public void JudgementOfWeakness(AbilitySO abilitySO)
    {
        currentJudgement = abilitySO;
        Debug.Log("Selected Judgement of Weakness");
    }

    public void HolyLight(AbilitySO abilitySO)
    {
        Debug.Log("Do Holy Light");
    }
}
