using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 1.5f; // Duration of the fade in seconds
    private float fadeTimer = 0f;
    private bool isFading = false;
    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            StartFade();
        }
        else
        {
            Debug.LogWarning("FadeOut: No Renderer found on this GameObject.");
        }
    }

    public void StartFade()
    {
        isFading = true;
        fadeTimer = 0f;
    }

    void Update()
    {
        if (!isFading || objectRenderer == null)
            return;

        fadeTimer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0f, fadeTimer / fadeDuration);

        Color newColor = originalColor;
        newColor.a = alpha;
        objectRenderer.material.color = newColor;

        if (fadeTimer >= fadeDuration)
        {
            isFading = false;
            gameObject.SetActive(false); // Hide the object
        }
    }
}