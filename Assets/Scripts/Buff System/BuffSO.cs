using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Buff", menuName = "Create Buff", order = 1)]
public class BuffSO : ScriptableObjectBase
{
    [FoldoutGroup("Info")]
    public int ID;
    [FoldoutGroup("Info")]
    public string buffName;
    [FoldoutGroup("Info")]
    [TextArea(10, 10)]
    public string buffDescription;
    [FoldoutGroup("Info")]
    public Sprite buffIcon;

    [FoldoutGroup("Properties")]
    public BuffType buffType;

    [FoldoutGroup("Properties")]
    public bool isDebuff;

    [FoldoutGroup("Properties")]
    public AttributeContainer buffbuffAttributes;  
    public override int GetID() => ID;

    public T GetBuffType<T>() where T : Enum
    {
        return (T)Enum.Parse(typeof(T), Utility.RemoveSpaceFromString(buffName));
    }
}

public enum BuffType
{
    Permanent,
    Temporary,
    Continuous
}

public enum BuffValueModifyType {Add, Subtract,Multiply,Divide,Set, Precentage};

[Serializable]
public class Buff
{
    public BuffSO buffSO;
    public GameUnit target;
    public Action OnBuffStart;
    public Action InBuffProgress;
    public Action OnBuffEnd;

    public event EventHandler OnBuffReset;
    public event EventHandler OnBuffRemoved;
    public static event EventHandler OnAnyBuffRemoved;

    public Buff(BuffSO buffSO, GameUnit target, Action onBuffStart, Action inProgress, Action onBuffEnd)
    {
        this.buffSO = buffSO;
        this.target = target;
        OnBuffStart = onBuffStart;
        InBuffProgress = inProgress;
        OnBuffEnd = onBuffEnd;
    }

    public void ResetBuff()
    {
        OnBuffReset?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveBuff()
    {
        OnBuffEnd?.Invoke();
        OnBuffRemoved?.Invoke(this, EventArgs.Empty);
        OnAnyBuffRemoved?.Invoke(this, EventArgs.Empty);
    }
}