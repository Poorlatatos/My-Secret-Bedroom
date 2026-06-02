using UnityEngine;
using System.Collections;

public class MobilePhoneAudioActivation : MonoBehaviour
{
    public Renderer phoneRenderer;
    public Color emissionColor = Color.yellow;
    public float buzzSpeed = 2f;
    public float emissionMin = 0.1f;
    public float emissionMax = 1.5f;

    [Header("Object To Show")]
    public GameObject objectToShow;
    public float displayTime = 26f;

    private bool hasBeenInteractedWith = false;
    private Material phoneMaterial;
    private float buzzTimer = 0f;

    private bool countdownStarted = false;

    void Start()
    {
        if (phoneRenderer == null)
            phoneRenderer = GetComponent<Renderer>();

        phoneMaterial = phoneRenderer.material;
        phoneMaterial.EnableKeyword("_EMISSION");

        if (objectToShow != null)
            objectToShow.SetActive(false);
    }

    void Update()
    {
        if (!hasBeenInteractedWith)
        {
            buzzTimer += Time.deltaTime * buzzSpeed;

            float emissionStrength = Mathf.Lerp(
                emissionMin,
                emissionMax,
                (Mathf.Sin(buzzTimer) + 1f) / 2f
            );

            phoneMaterial.SetColor("_EmissionColor", emissionColor * emissionStrength);
        }

        // Detect when object becomes active and start timer
        if (objectToShow != null && objectToShow.activeSelf && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(HideAfterDelay());
        }
    }

    public void OnInteract()
    {
        if (hasBeenInteractedWith)
            return;

        hasBeenInteractedWith = true;

        phoneMaterial.SetColor("_EmissionColor", Color.black);

        if (objectToShow != null)
        {
            objectToShow.SetActive(true);
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        if (objectToShow != null)
            objectToShow.SetActive(false);

        countdownStarted = false;
    }
}