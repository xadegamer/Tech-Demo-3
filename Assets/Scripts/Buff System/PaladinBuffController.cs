using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaladinBuffController : GameUnitBuffController
{
    protected override Buff CreateBuff(BuffSO buffSO, GameUnit target)
    {
        switch (buffSO.GetBuffType<PaladinBuff>())
        {
            case PaladinBuff.DevotionAura: return DevotionAura(buffSO, target);
            case PaladinBuff.MagicalAura: return MagicalAura(buffSO, target);
            case PaladinBuff.RetributionAura: return RetributionAura(buffSO, target);

            case PaladinBuff.SealOfRighteousness: return SealOfRighteousness(buffSO, target);
            case PaladinBuff.SealOfLight: return SealOfLight(buffSO, target);
            case PaladinBuff.SealOfJustice: return SealOfJustice(buffSO, target);

            case PaladinBuff.JudgementOfRighteousness: return JudgementOfWeakness(buffSO, target);
            case PaladinBuff.JudgementofWisdom: return JudgementOfWeakness(buffSO, target);
            case PaladinBuff.JudgementofWeakness: return JudgementOfWeakness(buffSO, target);
            default: return null;
        }
    }

    public Buff DevotionAura(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found DevotionalAuraBuff");

        float allDamageResistance = buffSO.buffbuffAttributes.GetAbilityValueByID("AllDamageResistance").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            target.HealthHandler.ModifyAllDamageResistance(allDamageResistance);
        }
        , null, () =>
        { // End
            target.HealthHandler.ModifyAllDamageResistance(allDamageResistance,false);
        });
    }
    public Buff MagicalAura(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found MagicalAuraBuff");
        
        float interval = buffSO.buffbuffAttributes.GetAbilityValueByID("Interval").GetValue<float>();
        float manaAmount = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().mana, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseMana").GetValue<float>());

        float timer = interval;

        return new Buff(buffSO, target, () =>
        { //Start

        }
        , () =>
        {// In Progress

            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                (gameUnit as PlayerUnit).PlayerStatHandler.GetManaValue().IncreaseValue(manaAmount);
                timer = interval;
            }

        }, () =>
        { // End

        });
    }
    public Buff RetributionAura(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found DevotionalAuraBuff");

        return new Buff(buffSO, target, () =>
        { //Start
             gameUnit.HealthHandler.OnHit.AddListener(damageInfo =>
            {
                float damage = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().maxbaseDamage, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseMeleeDamage").GetValue<float>());
                DamageInfo newDamageInfo = new DamageInfo();
                newDamageInfo.SetUp(DamageInfo.DamageType.Holy, damage, false, false);
                damageInfo.owner.HealthHandler.TakeDamage(newDamageInfo);
            });;
        }
        , null, () =>
        { // End
            gameUnit.HealthHandler.OnHit.RemoveAllListeners();
        });
    }
   
    public Buff SealOfLight(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found SealOfLight Buff");

        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            gameUnit.Damager.OnHit += damageDealth=>
            {
                if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("HealChance").GetValue<float>()))
                {
                    float health = Utility.CalculatePercentageOfValue(damageDealth, buffSO.buffbuffAttributes.GetAbilityValueByID("Heal").GetValue<float>());
                    gameUnit.HealthHandler.RestoreHealth(health);
                }
            };
        }
        , null, () =>
        { // End
            gameUnit.Damager.OnHit = null;
        });
    }
    public Buff SealOfJustice(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found SealOfJustice Buff");

        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        //float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start

            gameUnit.Damager.OnHitTargetHealth += healthHandler =>
            {
                if (Utility.CalculateChance(buffSO.buffbuffAttributes.GetAbilityValueByID("StunChance").GetValue<float>()))
                {
                    float stunDuration = buffSO.buffbuffAttributes.GetAbilityValueByID("StunDuration").GetValue<float>();
                    healthHandler.GetComponent<GameUnit>().Stun(stunDuration);
                }
            };          
        }
        , null, () =>
        { // End
            gameUnit.Damager.OnHitTargetHealth = null;
        });
    }
    public Buff SealOfRighteousness(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found SealOfRighteousness Buff");

        float duration = buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>();

        //float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start
            gameUnit.Damager.OnHitTargetHealth += healthHandler =>
            {
                float damage = Utility.CalculatePercentageOfValue(gameUnit.GetCharacterClassSO().maxbaseDamage, buffSO.buffbuffAttributes.GetAbilityValueByID("BaseWeaponDamage").GetValue<float>());
                DamageInfo newDamageInfo = new DamageInfo();
                newDamageInfo.SetUp(DamageInfo.DamageType.Holy, damage, false, false);
                if (healthHandler) healthHandler.TakeDamage(newDamageInfo);
            };
        }
        , () =>
        {// In Progress

            //if(timer > 0) timer -= Time.deltaTime;
            //else
            //{
            //    damageInfo.damageAmount = 5;
            //    target.HealthHandler.TakeDamage(damageInfo);
            //    timer = 2;
            //}

        }, () =>
        { // End
            gameUnit.Damager.OnHitTargetHealth = null;
        });
    }

    public Buff JudgementOfWeakness(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");

        float damageReduction = buffSO.buffbuffAttributes.GetAbilityValueByID("MeleeAttackDamageReduc").GetValue<float>();

        //float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.AddDamageReduction(damageReduction);
        }
        , null, () =>
        { // End
            target.Damager.RemoveDamageReduction(damageReduction);
        });
    }
}
