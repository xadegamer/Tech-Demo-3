using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float buffValue;
    [FoldoutGroup("Properties")]
    public float maxDuration;

    public override int GetID() => ID;
}


public enum BuffType
{
    Permanent,
    Temporary
}