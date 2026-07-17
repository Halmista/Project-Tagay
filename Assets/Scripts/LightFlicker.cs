using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.2f;
    public float maxIntensity = 2f;

    public float minInterval = 0.03f;
    public float maxInterval = 0.15f;

    private Light streetLight;
    private float timer;

    public bool IsFlickering { get; private set; }

    void Start()
    {
        streetLight = GetComponent<Light>();

        // Normal light at first
        streetLight.intensity = maxIntensity;
    }

    void Update()
    {
        if (!IsFlickering)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            streetLight.intensity =
                Random.Range(minIntensity, maxIntensity);

            SetNextInterval();
        }
    }

    void SetNextInterval()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    public void StartFlickering()
    {
        IsFlickering = true;
        SetNextInterval();
    }

    public void StopFlickering()
    {
        IsFlickering = false;
        streetLight.intensity = maxIntensity;
    }

    public IEnumerator ScareFlicker()
    {
        streetLight.enabled = false;
        yield return new WaitForSeconds(0.15f);

        streetLight.enabled = true;
        yield return new WaitForSeconds(0.08f);

        streetLight.enabled = false;
        yield return new WaitForSeconds(0.05f);

        streetLight.enabled = true;

        StartFlickering();
    }
}