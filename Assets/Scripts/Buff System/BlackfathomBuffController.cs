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
            default: return null;
        }
    }

    public override bool AddBuff(Buff newBuff)
    {
        if (base.AddBuff(newBuff))
        {
            UIManager.Instance.GetTargetUI().AddBuff(newBuff);
            return true;
        }
        return false;
    }

    public Buff BlackfathomHamstringDebuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found BlackfathomHamstringDebuff");

        float reduction = buffSO.buffbuffAttributes.GetAbilityValueByID("Movement and Attack Speed Reduction").GetValue<float>();

        return new Buff(buffSO, target, () =>
        { //Start
            ((PlayerUnit)target).MovementHandler.ModifySpeed(reduction, false);
            ((PlayerUnit)target).GetStat().ModifyAttackSpeed(reduction, true);
        }
        , () =>
        {// In Progress
            
        }, () =>
        { // End
            ((PlayerUnit)target).MovementHandler.ResetSpeed();
            ((PlayerUnit)target).GetStat().ResetAttackSpeed();
        });
    }

    public Buff BashDebuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");

        return new Buff(buffSO, target, () =>
        { //Start
            target.StartStun();
        }
        , () =>
        {// In Progress
            
        }, () =>
        { // End
            target.EndStun();
        });
    }

}
