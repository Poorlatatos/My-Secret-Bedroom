using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GrabObjectiveManager : MonoBehaviour
{
    public InteractionBehaviour[] interactionObjects;
    public ViewOnlyInteraction[] viewObjects;
    public GrabTracker[] objectsToGrab;

    [Header("Credits Canvas")]
    public GameObject creditsCanvas;

    private CanvasRenderer[] canvasRenderers;
    private bool loadedScene = false;

    void Start()
    {
        if (creditsCanvas != null)
        {
            canvasRenderers = creditsCanvas.GetComponentsInChildren<CanvasRenderer>();

            // Set initial alpha to 0 (fully invisible)
            foreach (CanvasRenderer r in canvasRenderers)
            {
                r.SetAlpha(0f);
            }

            creditsCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (loadedScene)
            return;

        foreach (InteractionBehaviour obj in interactionObjects)
        {
            if (!obj.hasBeenGrabbed)
                return;
        }

        foreach (ViewOnlyInteraction obj in viewObjects)
        {
            if (!obj.hasBeenViewed)
                return;
        }

        foreach (GrabTracker obj in objectsToGrab)
        {
            if (!obj.hasBeenGrabbed)
                return;
        }

        loadedScene = true;

        Debug.Log("All objectives completed! Showing credits...");
        StartCoroutine(ShowCreditsThenReturn());
    }

    IEnumerator ShowCreditsThenReturn()
    {
        // Wait BEFORE showing credits
        yield return new WaitForSeconds(10f);

        creditsCanvas.SetActive(true);

        // Fade + move settings
        float duration = 2f;
        float t = 0f;

        Vector3 startPos = creditsCanvas.transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.5f; // move up

        // Fade in + move up together
        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, t / duration);

            foreach (CanvasRenderer r in canvasRenderers)
            {
                r.SetAlpha(alpha);
            }

            creditsCanvas.transform.position = Vector3.Lerp(startPos, endPos, t / duration);

            yield return null;
        }

        // Ensure final state
        foreach (CanvasRenderer r in canvasRenderers)
        {
            r.SetAlpha(1f);
        }

        creditsCanvas.transform.position = endPos;

        // Wait during credits
        yield return new WaitForSeconds(15f);

        SceneManager.LoadScene(0);
    }
}