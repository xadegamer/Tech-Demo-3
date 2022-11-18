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
    public string buffDescription;
    [FoldoutGroup("Info")]
    public Sprite buffIcon;

    [FoldoutGroup("Properties")]
    public BuffType buffType;

    [FoldoutGroup("Properties")]
    public ValueDataContainer buffData;

    public Action OnBuffStart;
    public Action InBuffProgress;
    public Action OnBuffEnd;
    
    public override int GetID() => ID;

    public void SetActions(Action onBuffStart, Action inProgress, Action onBuffEnd)
    {
        OnBuffStart = onBuffStart;
        InBuffProgress = inProgress;
        OnBuffEnd = onBuffEnd;
    }
}

public enum BuffType
{
    Permanent,
    Temporary
}

public enum BuffValueModifyType {Add, Subtract,Multiply,Divide,Set, Precentage};