using UnityEngine;

public class LightFlickerScript : MonoBehaviour
{
    public Light flickerLight;
    public float minIntensity = 0f;
    public float maxIntensity = 0.9f;
    public float flickerIntervalMin = 0.05f;
    public float flickerIntervalMax = 0.2f;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();

        StartCoroutine(FlickerRoutine());
    }

    System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(Random.Range(flickerIntervalMin, flickerIntervalMax));
        }
    }
}
