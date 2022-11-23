using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;

    [SerializeField] float moveSpeed = 2f;

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

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
