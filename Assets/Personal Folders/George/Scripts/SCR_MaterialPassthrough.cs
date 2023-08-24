using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SCR_MaterialPassthrough : MonoBehaviour
{
    [SerializeField] private bool isHit;
    private bool isTransparent;
    [SerializeField] private bool isAnimating = false;

    Material material;

    [SerializeField] float tweenValue;
    bool destroyThis = false;

    SCR_MaterialPassthroughController controller;

    private void Awake()
    {
        controller = FindObjectOfType<SCR_MaterialPassthroughController>();
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (isHit && !isTransparent && !isAnimating)
        {
            isTransparent = true;
            LerpOpacity(1, controller.targetOpacity, 0.6f, 0f);
        }

        if (!controller.GetHitObjectsList().Contains(gameObject) && isTransparent && !isAnimating)
        {
            isHit = false;
            isTransparent = false;
            destroyThis = true;
            LerpOpacity(controller.targetOpacity, 1, 0.8f, 1f);
        }

        if (tweenValue == 1 && destroyThis)
        {
            Destroy(this);
        }
    }

    public void IsHit()
    {
        isHit = true;
    }

    void LerpOpacity(float start, float end, float time, float delay)
    {
        isAnimating = true;
        
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", end, "delay", delay,"easetype", iTween.EaseType.easeOutQuint, "time", time, "onupdate", "OnValueChanged", "oncomplete", "OnTweenCompleted"));
    }

    void OnValueChanged(float value)
    {
       material.SetFloat("_Opacity", value);
    }

    void OnTweenCompleted()
    {
        isAnimating = false;
    }
}
