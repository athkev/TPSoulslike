using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightBlinker : MonoBehaviour
{
    public float minIntensity = 5f;
    public float maxIntensity= 30f;
    public float blinkSpeed = 1f;
    Light[] lights;
    // Start is called before the first frame update
    void Start()
    {
        lights = GetComponentsInChildren<Light>();
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            // Calculate the lerp factor based on PingPong of time
            float lerpFactor = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);

            // Iterate through all lights and lerp their intensity
            foreach (Light light in lights)
            {
                light.intensity = Mathf.Lerp(minIntensity, maxIntensity, lerpFactor);
            }

            // Wait for the next frame
            yield return null;
        }
    }


}
