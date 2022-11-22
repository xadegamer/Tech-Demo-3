using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualiseText : MonoBehaviour
{
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Show(string text)
    {
        StartCoroutine(Visualise(text));
    }

    public IEnumerator Visualise(string message)
    {
        textMesh.text = "";
        textMesh.enabled = true;

        for (int i = 0; i < message.Length; i++)
        {
            textMesh.text += message[i];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);

        Hide();
    }

    public void Hide()
    {
        textMesh.enabled = false;
    }
}
