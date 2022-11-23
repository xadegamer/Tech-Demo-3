using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameUnitBuffController : MonoBehaviour
{
    public Action<BuffObject> OnBuffAdded;

    [SerializeField] protected Transform buffHolder;
    [SerializeField] protected BuffObject BuffObjectPrefab;
    [SerializeField] protected List<BuffObject> activeBuffObjects = new List<BuffObject>();

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

    public List<BuffObject> GetBuffObjects() => activeBuffObjects;

    public void SendBuff(BuffSO buffSO, GameUnit target)
    {
        if (target == null) return;
        Buff newBuffObject = CreateBuff(buffSO, target);
        target.GetComponent<GameUnitBuffController>().RecieveBuff(newBuffObject);
    }

    protected void Buff_OnAnyBuffRemoved(object sender, EventArgs e)
    {
        if(activeBuffObjects.Any(x => x.GetBuff() == sender as Buff))
        {
            activeBuffObjects.Remove(activeBuffObjects.First(x => x.GetBuff() == sender as Buff));
        }
    }

    public virtual void RemoveBuffs()
    {
        for (int i = 0; i < activeBuffObjects.Count; i++)
        {
            activeBuffObjects[i].GetBuff().RemoveBuff();
        }
    }

    protected abstract Buff CreateBuff(BuffSO buffSO, GameUnit target);

    public bool RecieveBuff(Buff newBuff)
    {
        BuffObject existingBuff = activeBuffObjects.FirstOrDefault(x => x.GetBuff().buffSO == newBuff.buffSO);

        if (existingBuff != null)
        {
            existingBuff.GetBuff().ResetBuff();
            return false;
        }

        BuffObject buffObject = Instantiate(BuffObjectPrefab, buffHolder);
        buffObject.ActivateBuff(newBuff);
        activeBuffObjects.Add(buffObject);
        OnBuffAdded?.Invoke(buffObject);

        return true;
    }
}
