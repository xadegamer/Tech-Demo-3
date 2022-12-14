using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffObjectUI : MonoBehaviour,IToolTip
{
    [Header("Buff UI")]
    [SerializeField] private Image abilityImage;
    [SerializeField] private TextMeshProUGUI coolDownText;

    [Header("Debug")]
    [SerializeField] private BuffObject buffObject;
    private Buff buff;
    private Vector2 pos;

    public void SetUp(BuffObject buffObject)
    {
        this.buffObject = buffObject;
        buff = buffObject.GetBuff();
        
        abilityImage.sprite = buffObject.GetBuff().buffSO.buffIcon;
        buffObject.GetBuff().OnBuffRemoved += Buff_OnBuffRemoved;
        buffObject.InProgress += UpdateTimerText;
        coolDownText.enabled = buffObject.GetBuff().buffSO.buffType == BuffType.Temporary;
    }

    public void UpdateTimerText(float time)
    {
        coolDownText.text = Utility.FloatToTime(time);
    }

    private void Buff_OnBuffRemoved(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    public void DeactivateBuff()
    {
        buffObject.DeactivateBuff();
    }

    public void OnClick()
    {
        pos = Input.mousePosition;
        ToolTipManager.Instance.ShowToolTip(this);
    }

    public string GetHeader() => buffObject.GetBuff().buffSO.buffName;

    public string GetContent() => buffObject.GetBuff().buffSO.buffDescription;

    public Sprite GetBackground() => null;

    public Color GetBackgroundColor() => Color.white;

    public Vector2 GetTocuchPositon() => pos;

    public Action GetAction() => buffObject.GetBuff().buffSO.isDebuff || buffObject.GetBuff().target is EnemyUnit  ? null : DeactivateBuff;

    private void OnDestroy()
    {
        if (buff != null) buff.OnBuffRemoved -= Buff_OnBuffRemoved;
    }
}
