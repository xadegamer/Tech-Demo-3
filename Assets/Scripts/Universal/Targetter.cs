using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Targetter : MonoBehaviour
{
    public static Targetter Instance { get; private set; }

    [SerializeField] private Color playerColour;
    [SerializeField] private Color enemyColour;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public static void SetTarget(GameUnit target)
    {
        if(target)
        {
            Instance.spriteRenderer.enabled = true;
            Instance.spriteRenderer.color = (target is PlayerUnit) ? Instance.playerColour : Instance.enemyColour;
            Instance.transform.SetParent(target.transform);
            Instance.transform.localPosition = Vector2.zero;
        }
        else
        {
            Instance.spriteRenderer.enabled = false;
        }
    }
}
