using UnityEngine;

public class EmissionFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Color emissionColor = Color.white;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 10.0f;

    private Material _material;
    private float _baseIntensity;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Use a unique instance of the material
            _material = renderer.material;
            _baseIntensity = 1.0f;
            _material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("EmissionFlicker: No Renderer found on this GameObject.");
        }
    }

    void Update()
    {
        if (_material == null) return;

        // Flicker using Perlin noise for smooth randomness
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        Color finalEmission = emissionColor * intensity;
        _material.SetColor(EmissionColorID, finalEmission);
    }
}
