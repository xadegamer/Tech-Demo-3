using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameUnitBuffController : MonoBehaviour
{
    [SerializeField] protected List<Buff> activeBuffs = new List<Buff>();

    protected virtual void Start()
    {
        BuffHolderUI.OnBuffHolderUIRemoved += BuffHolderUI_OnBuffHolderUIRemoved;
    }

    public void SendBuff(BuffSO buffSO, GameUnit target)
    {
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

    protected void BuffHolderUI_OnBuffHolderUIRemoved(object sender, EventArgs e)
    {
        if (activeBuffs.Contains((Buff)sender)) activeBuffs.Remove((Buff)sender);
    }
}
