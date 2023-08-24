using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_OpenDoor : MonoBehaviour
{
    [SerializeField] private float initialTorque;
    [SerializeField] private float angleFallOff;
    [SerializeField] private float speedFallOff;
    [SerializeField] private float swingDelay;

    [SerializeField] private float maxAngle;
    private Transform doorTransform;
    [SerializeField] private float originalYRot = 0;

    public bool isLocked = false;
    [SerializeField] private bool inCoroutine = false;
    [SerializeField] private bool isOpen = false;


    public void SmashOpenDoor(Vector3 playerDirection)
    {
        if (doorTransform == null)
        {
            doorTransform = transform;
            originalYRot = 0;
        }

        float dot = Vector3.Dot(doorTransform.forward, playerDirection);

        float newAngle = maxAngle;

        //If the door's forward and player's forward are facing same direction
        if (dot > 0)
        {
            newAngle = maxAngle * -1;
        }

        SCR_AudioManager.instance.Play("SFX_DoorSmash");
        StopAllCoroutines();
        StartCoroutine(RotateDoor(newAngle));
    }

    private IEnumerator RotateDoor(float newAngle)
    {
        inCoroutine = true;
        float currentAngle = newAngle;
        float currentTorque = initialTorque;
        Quaternion desiredRot = Quaternion.Euler(0, currentAngle + originalYRot, 0);
        while (doorTransform.localRotation != desiredRot)
        {
            doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, desiredRot, currentTorque * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(swingDelay);

        currentAngle *= angleFallOff * -1f;
        currentTorque *= speedFallOff;
        StartCoroutine(CloseDoor(currentAngle, currentTorque, desiredRot));
    }

    private IEnumerator CloseDoor(float currentAngle, float currentTorque, Quaternion desiredRot)
    {
        while (true)
        {
            desiredRot = Quaternion.Euler(0, currentAngle + originalYRot, 0);
            while (Quaternion.Angle(doorTransform.localRotation, desiredRot) > 1f)
            {
                doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, desiredRot, currentTorque * Time.deltaTime);
                yield return null;
            }
            currentTorque *= speedFallOff;
            currentAngle *= angleFallOff * -1f;

            if (currentTorque < 1f)
            {
                while (Quaternion.Angle(doorTransform.localRotation, Quaternion.Euler(0, originalYRot, 0)) > 1f)
                {
                    doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, Quaternion.Euler(0, originalYRot, 0), currentTorque * Time.deltaTime);
                    yield return null;
                }
                break;
            }
        }
        inCoroutine = false;
        isOpen = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) && !isLocked)
        {
            SmashOpenDoor(collision.transform.forward);
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    private void LateUpdate()
    {
        if (doorTransform == null) { return; }

        if (doorTransform.localEulerAngles.y != originalYRot)
        {
            isOpen = true;
        }
    }
}
