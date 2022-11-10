using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }
    public void DisableObject()
    {
        Destroy(gameObject);
    }
}
