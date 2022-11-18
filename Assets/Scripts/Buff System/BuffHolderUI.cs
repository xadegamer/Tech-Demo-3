using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuffHolderUI : MonoBehaviour
{
    public static event EventHandler OnBuffHolderUIAdded;
    public static event EventHandler OnBuffHolderUIRemoved;

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
        buff.OnBuffReset += ResetBuff;
        buff.OnBuffStart?.Invoke();
        buffCoroutine =  StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffData.GetAbilityValueByID("Duration").GetValue()));
    }

    public void DeactivateBuff()
    {
        StopCoroutine(buffCoroutine);
        RemoveBuff();
    }

    private void ResetBuff(object sender, EventArgs e)
    {
        StopCoroutine(buffCoroutine);
        buffCoroutine = StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffData.GetAbilityValueByID("Duration").GetValue()));
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
        RemoveBuff();
    }

    public void RemoveBuff()
    {
        buff.OnBuffEnd?.Invoke();
        OnBuffHolderUIRemoved?.Invoke(buff, EventArgs.Empty);
        Destroy(gameObject);
    }
}
