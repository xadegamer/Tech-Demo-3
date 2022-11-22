using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffHolderUI : MonoBehaviour,IToolTip
{
    [Header("Buff UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image coolDownSlider;
    [SerializeField] private TextMeshProUGUI coolDownText;
    
    [Header("Debug")]
    [SerializeField] private Buff buff;
    
    Coroutine buffCoroutine;
    Vector2 pos;


    public void ActivateBuff(Buff buff)
    {
        this.buff = buff;
        
        buff.OnBuffReset += Buff_ResetBuff;
        buff.OnBuffRemoved += Buff_OnBuffRemoved;

        buff.OnBuffStart?.Invoke();
        buffCoroutine =  StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>()));
    }

    private void Buff_ResetBuff(object sender, EventArgs e)
    {
        StopCoroutine(buffCoroutine);
        buffCoroutine = StartCoroutine(ActivateBuffRoutine(buff.buffSO.buffbuffAttributes.GetAbilityValueByID("Duration").GetValue<float>()));
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

    public void OnClick()
    {
        pos = Input.mousePosition;
        ToolTipManager.Instance.ShowToolTip(this);
    }
    
    public string GetHeader() => buff.buffSO.buffName;

    public string GetContent() => buff.buffSO.buffDescription;

    public Sprite GetBackground() => null;

    public Color GetBackgroundColor() => Color.white;

    public Vector2 GetTocuchPositon() => pos;

    public Action GetAction() => buff.buffSO.isDebuff ? null : DeactivateBuff;
}
