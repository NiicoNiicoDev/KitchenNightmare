using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_WeaponPickup : MonoBehaviour
{
    [SerializeField] float bounceHeight;
    [SerializeField] float interpolationTime;

    [SerializeField] int weaponHandlerWeaponsArrayIndex;

    Vector3 startingPosition;

    Vector3 posMax;
    Vector3 posMin;

    bool canPickUp = false;

    [SerializeField] private Transform canvas;
    private Transform cameraTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered");
            canPickUp = true;
            //Enable UI Element with keybind prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has exited");
            canPickUp = false;
            //Disable UI Element with keybind prompt
        }
    }

    private void Awake()
    {
        startingPosition = transform.position;
        startingPosition.y += bounceHeight;

        transform.position = startingPosition;

        posMax = transform.position;
        posMin = transform.position;

        posMax.y += bounceHeight * 0.5f;
        posMin.y -= bounceHeight * 0.5f;

        StartCoroutine(MoveUp());
    }

    private void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player picked up weapon");

            SCR_WeaponHandler weaponHandler = FindObjectOfType<SCR_WeaponHandler>();

            for (int i = 1; i < weaponHandler.EquippedWeapons.Length; i++)
            {
                if (weaponHandler.EquippedWeapons[i] == null)
                {
                    weaponHandler.UpdateEquippedWeapons(i, weaponHandlerWeaponsArrayIndex);
                    FindObjectOfType<SCR_WeaponUI>().UpdateWeaponUI(i);
                    Destroy(gameObject);
                }
            }
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        canvas.LookAt(cameraTransform, Vector3.up);
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.Euler(0, 10, 0), 5f * Time.deltaTime);
    }

    IEnumerator MoveUp()
    {
        float timePassed = 0f;
        Vector3 currentPosition = transform.position;

        while (timePassed <= interpolationTime)
        {
            transform.position = Vector3.Lerp(currentPosition, posMax, timePassed / interpolationTime);
            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = posMax;

        StartCoroutine(MoveDown());
    }

    IEnumerator MoveDown()
    {
        float timePassed = 0f;
        Vector3 currentPosition = transform.position;

        while (timePassed <= interpolationTime)
        {
            transform.position = Vector3.Lerp(currentPosition, posMin, timePassed / interpolationTime);
            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = posMin;

        StartCoroutine(MoveUp());
    }
}
