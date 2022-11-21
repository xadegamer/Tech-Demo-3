using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAbilityController : GameUnitAbilityController
{
    [SerializeField] AbilitySO currentJudgement;

    [SerializeField] AbilitySO currentAura;

    [SerializeField] AbilitySO currentSeal;

    Coroutine sealCoroutine;

    private bool sealActive;

    protected void Start()
    {
        AbilityUIManager.Instance.SetAbilities(abilitySetSOArray);

        currentAura.UseAbility();
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

                case PaladinAbilities.SealOfRighteousness: abilitySO.SetAbilityAction(StartSealOfRighteousness, EndSealOfRighteousness); break;
                case PaladinAbilities.SealOfLight: abilitySO.SetAbilityAction(StartSealOfLight, EndSealOfLight); break;
                case PaladinAbilities.SealOfJustice: abilitySO.SetAbilityAction(StartSealOfJustice, EndSealOfJustice); break;

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

    #region Set 1
    public void CrusaderStrike(AbilitySO abilitySO)
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
    #endregion

    #region Set 4

    #region Aura
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
    #endregion

    #region Seal

    public void StartSealOfRighteousness(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfRighteousness");

        if(currentSeal != null && sealActive) currentSeal.EndAbility();
        currentSeal = abilitySO;
        sealActive = true;

        if (sealCoroutine != null) StopCoroutine(sealCoroutine);
        float duration = abilitySO.abilityData.GetAbilityValueByID("Duration").GetValue();
        sealCoroutine = StartCoroutine(Utility.TimedAbility(abilitySO ,() => Debug.Log("Start SealOfRighteousness"), duration, EndSealOfRighteousness));

        Debug.Log(duration);
    }

    public void EndSealOfRighteousness(AbilitySO abilitySO)
    {
        sealActive = false;
        Debug.Log("end SealOfRighteousness");
    }

    public void StartSealOfLight(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfLight");
        if (currentSeal != null && sealActive) currentSeal.EndAbility();
        currentSeal = abilitySO;
        sealActive = true;

        if (sealCoroutine != null) StopCoroutine(sealCoroutine);
        float duration = abilitySO.abilityData.GetAbilityValueByID("Duration").GetValue();
        sealCoroutine = StartCoroutine(Utility.TimedAbility(abilitySO ,() => Debug.Log("Start StartSealOfLight"), duration, EndSealOfLight));
    }

    public void EndSealOfLight(AbilitySO abilitySO)
    {
        Debug.Log("end StartSealOfLight");
    }

    public void StartSealOfJustice(AbilitySO abilitySO)
    {
        Debug.Log("Do SealOfJustice");
        if (currentSeal != null && sealActive) currentSeal.EndAbility();
        currentSeal = abilitySO;
        sealActive = true;

        if (sealCoroutine != null) StopCoroutine(sealCoroutine);
        float duration = abilitySO.abilityData.GetAbilityValueByID("Duration").GetValue();
        sealCoroutine = StartCoroutine(Utility.TimedAbility(abilitySO ,() => Debug.Log("Start StartSealOfJustice"), duration, EndSealOfJustice));
    }

    public void EndSealOfJustice(AbilitySO abilitySO)
    {
        Debug.Log("end StartSealOfJustice");
    }
    #endregion

    #region Judgement
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
    #endregion

    #region Blessing
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
    #endregion
    #endregion
}
