using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform toolTipUI;
    [SerializeField] private Image toolTipBg;
    [SerializeField] private Sprite defaultBg;
    [SerializeField] private Color bgColor;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWarpLimit;
    [SerializeField] private float waitTime;
    [SerializeField] private GameObject actionButton;

    Action action;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowToolTip(IToolTip objectTouched)
    {
        SetText(objectTouched.GetContent(), objectTouched.GetHeader(), objectTouched.GetTocuchPositon(), objectTouched.GetAction());
        SetBackgroundImage(objectTouched.GetBackground(), objectTouched.GetBackgroundColor());
    }

    public void SetText(string content, string header = "", Vector2 pos = default, Action action = null)
    {
        contentField.gameObject.SetActive(!string.IsNullOrEmpty(content));
        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));

        headerField.text = header;
        contentField.text = content;

        int headerLenth = headerField.text.Length;
        int contentLenght = contentField.text.Length;
        
        layoutElement.enabled = headerLenth > characterWarpLimit || contentLenght > characterWarpLimit;

        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;

        toolTipUI.pivot = new Vector2(pivotX, pivotY);
        toolTipUI.transform.position = pos;
        toolTipUI.gameObject.SetActive(true);

        if (action == null)  actionButton.SetActive(false);
        else
        {
            this.action = action;
            actionButton.SetActive(true);
        }
    }

    public void DoActon()
    {
        action?.Invoke();
        action = null;
    }

    IEnumerator SetTextCoroutine(string content, string header = "", Vector2 pos = default)
    {
        yield return new WaitForSeconds(1f);
        SetText(content, header);
    }


    public void SetBackgroundImage(Sprite sprite, Color color)
    {
        toolTipBg.sprite = sprite != null ? sprite : defaultBg;
        toolTipBg.color = bgColor;
    }
}

public interface IToolTip
{
    public string GetHeader();
    public string GetContent();
    public Sprite GetBackground();
    public Color GetBackgroundColor();
    public Vector2 GetTocuchPositon();
    public Action GetAction();
}
