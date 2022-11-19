using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuffHolderUI : MonoBehaviour
{
    [Header("Buff UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI coolDownText;

    [Header("Buff Option")]
    [SerializeField] private Buff buff;
    
    Coroutine buffCoroutine;

    public void ActivateBuff(Buff buff)
    {
        this.buff = buff;
        
        buff.OnBuffReset += Buff_ResetBuff;
        buff.OnBuffRemoved += Buff_OnBuffRemoved;

        buff.OnBuffStart?.Invoke();
        buffCoroutine =  StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffData.GetAbilityValueByID("Duration").GetValue()));
    }

    private void Buff_ResetBuff(object sender, EventArgs e)
    {
        StopCoroutine(buffCoroutine);
        buffCoroutine = StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffData.GetAbilityValueByID("Duration").GetValue()));
    }

    private void Buff_OnBuffRemoved(object sender, EventArgs e)
    {
        Destroy(gameObject);
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

    public IEnumerator ActivateBuffRoutine(float duration)
    {
        float startDuration = duration;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            coolDownText.text = Utility.FloatToTime(duration);
          //  coolDownSlider.fillAmount = duration / startDuration;
            buff.InBuffProgress?.Invoke();
            yield return null;
        }
        buff.RemoveBuff();
    }
}
