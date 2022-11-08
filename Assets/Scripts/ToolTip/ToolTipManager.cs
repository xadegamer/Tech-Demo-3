using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private RectTransform toolTipUI;
    [SerializeField] private Image toolTipBg;
    [SerializeField] private Sprite defaultBg;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWarpLimit;
    [SerializeField] float showTimer;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowToolTip(IToolTip objectTouched)
    {
        SetText(objectTouched.GetContent(), objectTouched.GetHeader());
        SetBackgroundImage(objectTouched.GetBackground(), objectTouched.GetBackgroundColor());
    }

    public void SetText(string content, string header = "")
    {
        contentField.gameObject.SetActive(!string.IsNullOrEmpty(content));
        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));

        headerField.text = header;
        contentField.text = content;

        int headerLenth = headerField.text.Length;
        int contentLenght = contentField.text.Length;
        layoutElement.enabled = headerLenth > characterWarpLimit || contentLenght > characterWarpLimit;

        toolTipUI.gameObject.SetActive(true);
    }


    public void SetBackgroundImage(Sprite sprite, Color color)
    {
        toolTipBg.sprite = sprite != null ? sprite : defaultBg;
        toolTipBg.color = color;
    }
}

public interface IToolTip
{
    public string GetHeader();
    public string GetContent();
    public Sprite GetBackground();
    public Color GetBackgroundColor();
}
