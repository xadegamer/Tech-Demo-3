using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpTextManager : MonoBehaviour
{
    public static PopUpTextManager Instance { get; private set; }

    [Header("PopUp")]
    [SerializeField] private FloatingText  floatingText;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 randomIntensity;

    void Awake() 
    { 
        Instance = this; 
    }

    public void PopUpText (Transform spawnPosition, string text, Color color)
    {
        FloatingText textPopUp = Instantiate(floatingText, spawnPosition.position + (Vector3)offset, Quaternion.identity, transform);
        textPopUp.transform.position += new Vector3( Random.Range(-randomIntensity.x, randomIntensity.x), Random.Range(-randomIntensity.x, randomIntensity.y),transform.localPosition.z);
        textPopUp.SetText(text, color);
    }
}
