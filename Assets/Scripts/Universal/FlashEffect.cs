using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AllIn1SpriteShader;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] private float duration;

    private Material mat;
    
    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }
    
    IEnumerator FlashRoutine()
    {
        float dissolveValue = 1f;
        while (dissolveValue > 0f)
        {
            dissolveValue -= Time.deltaTime / duration;
            dissolveValue = Mathf.Clamp01(dissolveValue);
            mat.SetFloat("_HitEffectBlend", dissolveValue);
            yield return null;
        }
    }

    public void ToggleFlash(bool toggle)
    {
        mat.EnableKeyword(toggle ? "HITEFFECT_ON" : "HITEFFECT_OFF");
    }

    public void ToggleNegative(bool toggle)
    {
        mat.EnableKeyword(toggle ? "NEGATIVE_ON" : "NEGATIVE_OFF");
    }
}
