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

    private Transform vrCamera;

    public GameObject leftController;
    public GameObject rightController;

    void Start()
    {
        vrCamera = Camera.main.transform;

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
        yield return new WaitForSeconds(10f);

        creditsCanvas.SetActive(true);
        creditsCanvas.transform.SetParent(vrCamera);
        creditsCanvas.transform.localPosition = new Vector3(0f, 0f, 0.905f);
        creditsCanvas.transform.localRotation = Quaternion.identity;

        // 🔥 HIDE VR CONTROLLERS
        if (leftController != null)
            leftController.SetActive(false);

        if (rightController != null)
            rightController.SetActive(false);

        float duration = 2f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, t / duration);

            foreach (CanvasRenderer r in canvasRenderers)
            {
                r.SetAlpha(alpha);
            }

            yield return null;
        }

        foreach (CanvasRenderer r in canvasRenderers)
        {
            r.SetAlpha(1f);
        }

        yield return new WaitForSeconds(15f);

        SceneManager.LoadScene(0);
    }
}