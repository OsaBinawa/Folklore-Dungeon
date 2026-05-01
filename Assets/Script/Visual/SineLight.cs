using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SineLight : MonoBehaviour
{
    public float baseIntensity = 1f;  
    public float amplitude = 1f;
    public float frequency = 1f;       

    private Light2D light2D;

    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        float sineValue = Mathf.Sin(Time.time * frequency);
        float remapped = (sineValue + 1f) * 0.5f;
        light2D.intensity = baseIntensity + (amplitude * remapped);
    }
}
