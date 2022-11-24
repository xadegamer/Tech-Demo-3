using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffObject : MonoBehaviour
{
    public Action<float> InProgress;
    
    [SerializeField] private Buff buff;
    Coroutine buffCoroutine;

    public void ActivateBuff(Buff buff)
    {
        this.buff = buff;
        buff.OnBuffReset += Buff_ResetBuff;
        buff.OnBuffRemoved += Buff_OnBuffRemoved;
        buff.OnBuffStart?.Invoke();

        switch (buff.buffSO.buffType)
        {
            case BuffType.Permanent:

                break;
            case BuffType.Continuous:
                buffCoroutine = StartCoroutine(Utility.IntervalAbility(null, buff.buffSO.buffbuffAttributes.GetAbilityValueByID("Interval").GetValue<float>(), BuffActiveAction));
                break;
            case BuffType.Temporary:
                buffCoroutine = StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>()));
                break;
            default: break;
        }
    }

    public void BuffActiveAction()
    {
        buff.InBuffProgress?.Invoke();
    }  
    
    public IEnumerator ActivateBuffRoutine(float duration)
    {
        float startDuration = duration;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            InProgress?.Invoke(duration);
            BuffActiveAction();
            yield return null;
        }
        buff.RemoveBuff();
    }

    public void DeactivateBuff()
    {
        StopCoroutine(buffCoroutine);
        buff.RemoveBuff();
    }

    private void RemoveBuff(object sender, EventArgs e)
    {
        DeactivateBuff();
    }

    private void Buff_ResetBuff(object sender, EventArgs e)
    {
        if (buffCoroutine != null) StopCoroutine(buffCoroutine);
        buffCoroutine = StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>()));
    }

    private void Buff_OnBuffRemoved(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    public Buff GetBuff()
    {
        return buff;
    }
}
