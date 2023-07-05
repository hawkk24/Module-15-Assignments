using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    private Transform cameraTransform;
    private bool isShaking = false;
    private float shakeDuration = 0.33f;
    private float shakeMagnitude = 2f;

    public AnimationCurve animCurve;

    void Awake()
    {
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            isShaking = false;
            StartCoroutine(shake());
        }
    }

    public void triggerShake()
    {
        isShaking = true;
    }

    private IEnumerator shake()
    {
        Vector3 initialPosition = cameraTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentMagnitude = shakeMagnitude * (animCurve.Evaluate(elapsedTime / shakeDuration));
            cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * currentMagnitude;
            yield return null;
        }

        cameraTransform.localPosition = initialPosition;
    }
}
