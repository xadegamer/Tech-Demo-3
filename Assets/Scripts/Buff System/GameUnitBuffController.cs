using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameUnitBuffController : MonoBehaviour
{
    [SerializeField] protected List<Buff> activeBuffs = new List<Buff>();

    protected GameUnit gameUnit;
    protected DamageInfo damageInfo;

    private void Awake()
    {
        gameUnit = GetComponent<GameUnit>();
        damageInfo = new DamageInfo();
        damageInfo.owner = gameUnit;
    }

    protected virtual void Start()
    {
        Buff.OnAnyBuffRemoved += Buff_OnAnyBuffRemoved;
    }

    public void SendBuff(BuffSO buffSO, GameUnit target)
    {
        if (target == null) return;
        Buff newBuff = CreateBuff(buffSO, target);
        target.GetComponent<GameUnitBuffController>().AddBuff(newBuff);
    }

    protected abstract Buff CreateBuff(BuffSO buffSO, GameUnit target);

    public virtual bool AddBuff(Buff newBuff)
    {
        Buff existingBuff = activeBuffs.FirstOrDefault(x => x.buffSO == newBuff.buffSO);

        if (existingBuff != null)
        {
            existingBuff.ResetBuff();
            return false;
        }
        
        activeBuffs.Add(newBuff);
        return true;
    }

    protected void Buff_OnAnyBuffRemoved(object sender, EventArgs e)
    {
        if (activeBuffs.Contains((Buff)sender)) activeBuffs.Remove((Buff)sender);
    }

    public virtual void RemoveBuffs()
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].RemoveBuff();
        }
    }
}
