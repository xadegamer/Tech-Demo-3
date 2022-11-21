using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : GameUnitAbilityController
{
    [SerializeField] AbilitySO currentJudgement;

    [SerializeField] AbilitySO currentAura;

    [SerializeField] AbilitySO currentSeal;

    protected void Start()
    {
        AbilityUIManager.Instance.SetAbilities(abilitySetSOArray);

        
    }

    protected override void AssignSetAbilityActions(AbilitySetSO abilitySetSO)
    {
        foreach (AbilitySO abilitySO in abilitySetSO.abilities)
        {
            switch (abilitySO.GetAbilityType<PaladinAbilities>())
            {
                case PaladinAbilities.CrusaderStrike: abilitySO.SetAbilityAction(CrusaderStrike); break;
                case PaladinAbilities.HammerofJustice: abilitySO.SetAbilityAction(HammerofJustice); break;
                case PaladinAbilities.DivineStorm: abilitySO.SetAbilityAction(DivineStorm); break;
                case PaladinAbilities.Judgement: abilitySO.SetAbilityAction(Judgement); break;


                case PaladinAbilities.DevotionalAura: abilitySO.SetAbilityAction(DevotionalAura); break;
                case PaladinAbilities.MagicalAura: abilitySO.SetAbilityAction(MagicalAura); break;
                case PaladinAbilities.RetributionAura: abilitySO.SetAbilityAction(RetributionAura); break;

                case PaladinAbilities.SealOfRighteousness: abilitySO.SetAbilityAction(SealOfRighteousness); break;
                case PaladinAbilities.SealOfLight: abilitySO.SetAbilityAction(SealOfLight); break;
                case PaladinAbilities.SealOfJustice: abilitySO.SetAbilityAction(SealOfJustice); break;


                case PaladinAbilities.JudgementOfRighteousness: abilitySO.SetAbilityAction(JudgementOfRighteousness); break;
                case PaladinAbilities.JudgementofWisdom: abilitySO.SetAbilityAction(JudgementOfWisdom); break;
                case PaladinAbilities.JudgementofWeakness: abilitySO.SetAbilityAction(JudgementOfWeakness); break;

                case PaladinAbilities.BlessingOfMight: abilitySO.SetAbilityAction(BlessingOfMight); break;
                case PaladinAbilities.BlessingOfWisdom: abilitySO.SetAbilityAction(BlessingOfWisdom); break;
                case PaladinAbilities.BlessingOfKings: abilitySO.SetAbilityAction(BlessingOfKings); break;

                default: break;
            }
        }
    }

    public void CrusaderStrike(AbilitySO  abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);
        
        PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);

        Debug.Log("Did Crusader Strike : " + damage);
    }
    
    public void HammerofJustice(AbilitySO abilitySO)
    {
        float stunDuration = abilitySO.abilityData.GetAbilityValueByID("StunDuration").GetValue();
        Debug.Log("Did Hammer of Justice : " + stunDuration);
    }

    public void DivineStorm(AbilitySO abilitySO)
    {
        float damage = Utility.CalculateValueWithPercentage(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BasePhysicalDamage").GetValue(), true);

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);

        float healAmount = Utility.CalculatePercentageOfValue(PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo), abilitySO.abilityData.GetAbilityValueByID("Heal").GetValue());
        Debug.Log("Did Divine Storm : " + damage);
        Debug.Log("Heal : " + healAmount);


        GetComponent<HealthHandler>().RestoreHealth(healAmount);
    }

    public void Judgement(AbilitySO abilitySO)
    {
        float damage = Utility.CalculatePercentageOfValue(gameUnit.GetStat().GetCharacterClassSO().minbaseDamage, abilitySO.abilityData.GetAbilityValueByID("BaseWeaponDamage").GetValue());

        damageInfo.SetUp(DamageInfo.DamageType.Melee, damage, false, false);
        PlayerUnit.Instance.GetTarget().GetComponent<HealthHandler>().TakeDamage(damageInfo);
        Debug.Log("Did Judgement : " + damage);

        buffManager.SendBuff(currentJudgement.buff, PlayerUnit.Instance.GetTarget());
        Debug.Log("Do " + currentJudgement.abilityName);
    }

    public void DevotionalAura(AbilitySO abilitySO)
    {
        Debug.Log("Did Devotion Aura ");
    }

    public void MagicalAura(AbilitySO abilitySO)
    {
        Debug.Log("Do MagicalAurat");
    }

    public void RetributionAura(AbilitySO abilitySO)
    {
        Debug.Log("Do RetributionAura");
    }

    public void SealOfRighteousness(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfRighteousness");
    }

    public void SealOfLight(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfLight");
    }

    public void SealOfJustice(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfJustice");
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

    public void BlessingOfMight(AbilitySO abilitySO)
    {
        Debug.Log("Do BlessingOfMight");
    }

    public void BlessingOfWisdom(AbilitySO abilitySO)
    {
        Debug.Log("Do BlessingOfWisdom");
    }

    public void BlessingOfKings(AbilitySO abilitySO)
    {
        Debug.Log("Do BlessingOfKings");
    }
}
