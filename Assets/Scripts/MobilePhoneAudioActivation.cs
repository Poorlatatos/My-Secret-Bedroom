using UnityEngine;

public class MobilePhoneAudioActivation : MonoBehaviour
{
    public Renderer phoneRenderer; // Assign in Inspector or get in Awake
    public Color emissionColor = Color.yellow;
    public float buzzSpeed = 2f;
    public float emissionMin = 0.1f;
    public float emissionMax = 1.5f;

    private bool hasBeenInteractedWith = false;
    private Material phoneMaterial;
    private float buzzTimer = 0f;

    void Start()
    {
        if (phoneRenderer == null)
            phoneRenderer = GetComponent<Renderer>();

        phoneMaterial = phoneRenderer.material;
        phoneMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!hasBeenInteractedWith)
        {
            buzzTimer += Time.deltaTime * buzzSpeed;
            float emissionStrength = Mathf.Lerp(emissionMin, emissionMax, (Mathf.Sin(buzzTimer) + 1f) / 2f);
            phoneMaterial.SetColor("_EmissionColor", emissionColor * emissionStrength);
        }
    }

    // Call this method when the object is interacted with
    public void OnInteract()
    {
        hasBeenInteractedWith = true;
        phoneMaterial.SetColor("_EmissionColor", Color.black); // Optionally turn off emission
    }
}