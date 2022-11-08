using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour, IToolTip
{
    [Header("ToolTip Text")]
    [SerializeField] private string header;
    [SerializeField] private string content;

    [Header("ToolTip Background")]
    [SerializeField] private Sprite background;
    [SerializeField] private Color backgroundColor =  Color.black;

    public Color GetBackgroundColor()
    {
        return backgroundColor;
    }
    
    public Sprite GetBackground()
    {
        return background;
    }

    public string GetContent()
    {
       return content;
    }

    public string GetHeader()
    {
        return header;
    }
}
