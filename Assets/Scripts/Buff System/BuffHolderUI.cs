using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuffHolderUI : MonoBehaviour
{
    [Header("Buff UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;

    [Header("Buff Option")]
    [SerializeField] private BuffSO buffSO;
    
    Coroutine buffCoroutine;

    public void ActivateBuff(BuffSO buffSO)
    {
        this.buffSO = buffSO;
        float duration = buffSO.buffData.GetAbilityValueByID("Duration").GetValue();
        buffCoroutine = StartCoroutine(ActivateBuffRoutine(duration));
    }

    public void DeactivateBuff()
    {
        StopCoroutine(buffCoroutine);
        buffSO.OnBuffEnd?.Invoke();
        Destroy(gameObject);
    }

    public IEnumerator ActivateBuffRoutine(float duration)
    {
        buffSO.OnBuffStart?.Invoke();
        float startDuration = duration;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            coolDownSlider.fillAmount = duration / startDuration;
            buffSO.InBuffProgress?.Invoke();
            yield return null;
        }
        buffSO.OnBuffEnd?.Invoke();
        Destroy(gameObject);
    }
}
