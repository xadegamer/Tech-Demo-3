using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackfathomBuffController : GameUnitBuffController
{
    protected override Buff CreateBuff(BuffSO buffSO, GameUnit target)
    {
        switch (buffSO.GetBuffType<BlackfathomBuff>())
        {
            case BlackfathomBuff.BlackfathomHamstringDebuff: return BlackfathomHamstringDebuff(buffSO, target);
            case BlackfathomBuff.BashDebuff: return BashDebuff(buffSO, target);
            case BlackfathomBuff.NagaSpiritBuff: return NagaSpiritBuff(buffSO, target);
            default: return null;
        }
    }

    public Buff BlackfathomHamstringDebuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found BlackfathomHamstringDebuff");

        float reduction = buffSO.buffbuffAttributes.GetAbilityValueByID("Movement and Attack Speed Reduction").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            ((PlayerUnit)target).MovementHandler.ModifySpeed(reduction, false);
            target.GetStat().ModifyAttackSpeed(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().attackSpeed, reduction), true);
        }
        , null, () =>
        { // End
            ((PlayerUnit)target).MovementHandler.ResetSpeed();
            target.GetStat().ModifyAttackSpeed(Utility.CalculatePercentageOfValue(target.GetCharacterClassSO().attackSpeed, reduction), false);
        });
    }

    public Buff BashDebuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");

        return new Buff(buffSO, target, () =>
        { //Start
            target.StartStun();
        }
        , null, () =>
        { // End
            target.EndStun();
        });
    }
    
    public Buff NagaSpiritBuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found NagaSpiritBuff");
        float healPercentage = buffSO.buffbuffAttributes.GetAbilityValueByID("HealPercentage").GetValue<float>();
        float healthPerSecond = buffSO.buffbuffAttributes.GetAbilityValueByID("HealthPerSecond").GetValue<float>();

        float healAmount = Utility.CalculatePercentageOfValue((target as EnemyUnit).LastCriticalDamageDealth(), healPercentage);
        float timer = 0;

        return new Buff(buffSO, target, null
        , () =>
        {// In Progress

            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                target.HealthHandler.RestoreHealth(Utility.CalculatePercentageOfValue(healAmount, healthPerSecond));
                timer = 1;
            }

        },null);
    }
}
