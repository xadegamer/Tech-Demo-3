using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Buff
{
    public BuffSO buffSO;
    public float currentDuration;
    public Action<float> OnBuffActive;
    public Action OnBuffEnd;

    public Buff(BuffSO buffSO, Action<float> buffEffect)
    {
        this.buffSO = buffSO;
        this.OnBuffActive = buffEffect;
        currentDuration = buffSO.maxDuration;
    }

    public float GetNormalisedDuration() => currentDuration / buffSO.maxDuration;

    public void ActivateBuff(Action buffEffect)
    {
        buffEffect?.Invoke();

        if (buffSO.buffType == BuffType.Temporary) ReduceBuffCurrentDuration();
    }

    IEnumerator ReduceBuffCurrentDuration()
    {
        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            yield return null;
        }

        currentDuration = 0;

        OnBuffEnd?.Invoke();
    }
}
