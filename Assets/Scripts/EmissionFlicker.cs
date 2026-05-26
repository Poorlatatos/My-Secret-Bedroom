using UnityEngine;

public class EmissionFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Color emissionColor = Color.white;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 10.0f;

    [Header("Material Swap Settings")]
    public Material[] materials;   // List of materials to cycle through
    public float swapInterval = 5f;

    private Material _material;
    private Renderer _renderer;
    private int _currentMaterialIndex = 0;

    private float _swapTimer;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            // Create unique material instance
            _material = _renderer.material;

            _material.EnableKeyword("_EMISSION");

            // Set first material if list exists
            if (materials.Length > 0)
            {
                _renderer.material = materials[0];
                _material = _renderer.material;
            }
        }
        else
        {
            Debug.LogWarning("EmissionFlicker: No Renderer found on this GameObject.");
        }
    }

    void Update()
    {
        if (_material == null) return;

        // ---------------- FLICKER ----------------
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        Color finalEmission = emissionColor * intensity;
        _material.SetColor(EmissionColorID, finalEmission);

        // ---------------- MATERIAL SWAP ----------------
        if (materials.Length > 1)
        {
            _swapTimer += Time.deltaTime;

            if (_swapTimer >= swapInterval)
            {
                _swapTimer = 0f;

                // Go to next material
                _currentMaterialIndex++;

                if (_currentMaterialIndex >= materials.Length)
                {
                    _currentMaterialIndex = 0;
                }

                // Apply material
                _renderer.material = materials[_currentMaterialIndex];

                // Update reference
                _material = _renderer.material;

                // Make sure emission stays enabled
                _material.EnableKeyword("_EMISSION");
            }
        }
    }
}