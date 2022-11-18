using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();

    private void Start()
    {
        BuffHolderUI.OnBuffHolderUIRemoved += BuffHolderUI_OnBuffHolderUIRemoved;
    }

    public void SendBuff(BuffSO buffSO, GameUnit target)
    {
        Buff newBuff = CreateBuff(buffSO, target);
        target.GetComponent<BuffManager>().AddBuff(newBuff);
    }

    public Buff CreateBuff(BuffSO buffSO, GameUnit target)
    {
        switch (buffSO.GetBuffType<PaladinBuff>())
        {
            case PaladinBuff.JudgementOfRighteousness: return JudgementOfWeaknessBuff(buffSO, target);
            case PaladinBuff.JudgementofWisdom: return JudgementOfWeaknessBuff(buffSO, target);
            case PaladinBuff.JudgementofWeakness: return JudgementOfWeaknessBuff(buffSO, target);
            default: return null;
        } 
    }

    public Buff JudgementOfWeaknessBuff(BuffSO buffSO, GameUnit target)
    {
        Debug.Log("Found JudgementOfWeaknessBuff");
        
        float damageReduction = buffSO.buffData.GetAbilityValueByID("MeleeAttackDamage").GetValue();
        
        DamageInfo damageInfo = new DamageInfo();

        float timer = 2;

        return new Buff(buffSO, target, () =>
        { //Start
            target.Damager.SetDamageReducion(damageReduction);
        }
        , () =>
        {// In Progress
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                damageInfo.damageAmount = 5;
                target.HealthHandler.TakeDamage(damageInfo);
                timer = 2;
            }
        }, () =>
        { // End
            target.Damager.SetDamageReducion(0f);
        });
    }

    private void AddBuff(Buff newBuff)
    {
        Buff existingBuff = activeBuffs.FirstOrDefault(x => x.buffSO == newBuff.buffSO);

        if (existingBuff != null) 
        {
            existingBuff.ResetBuff();
            return;
        }

        activeBuffs.Add(newBuff);
        UIManager.Instance.SpawnPlayerBuffUI(newBuff);
    }

    private void BuffHolderUI_OnBuffHolderUIRemoved(object sender, EventArgs e)
    {
        if (activeBuffs.Contains((Buff)sender)) activeBuffs.Remove((Buff)sender);
    }
}
