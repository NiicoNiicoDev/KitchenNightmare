using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve;

    //Code adapted from Thomas Friday, 2021
    private IEnumerator CameraShake(float desiredTime, float intensity)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < desiredTime)
        {
            elapsedTime += Time.deltaTime;
            float strength = animCurve.Evaluate(elapsedTime / desiredTime) * intensity;
            transform.position += Random.insideUnitSphere * strength;
            yield return null;
        }
    }

    //End of adapted code

    public void CallCameraShake(float desiredTime, float intensity)
    {
        StartCoroutine(CameraShake(desiredTime, intensity));
    }
}
