using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractionBehaviour : MonoBehaviour
{
    public Camera vrCamera;
    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;

    public TMP_Text infoTextUI;

    // UI IMAGE PANEL
    public GameObject infoPanelUI;

    public GameObject blackScreenOverlay;
    public string infoTextFilePath = "Assets/InfoText.txt";

    public TypewriterEffect typewriterEffect;
    public QuestionToAnswerScript questionToAnswerScript;
    private float originalFOV;
    private bool isTriggered = false;
    private Transform targetObject;

    public bool hasBeenGrabbed = false;

    private Dictionary<string, string> infoDict = new Dictionary<string, string>();

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        originalFOV = vrCamera.fieldOfView;

        LoadInfoText();

        if (infoTextUI != null)
            infoTextUI.text = "";

        // Get interactable component
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // Subscribe to XR events
        simpleInteractable.selectEntered.AddListener(OnSelectEnter);
        simpleInteractable.selectExited.AddListener(OnSelectExit);
    }

    void Update()
    {
        if (isTriggered && targetObject != null)
        {
            // Look at object
            Vector3 direction = (targetObject.position - vrCamera.transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            vrCamera.transform.rotation = Quaternion.Slerp(
                vrCamera.transform.rotation,
                lookRotation,
                Time.deltaTime * lockOnSpeed
            );

            // Zoom in
            vrCamera.fieldOfView = Mathf.Lerp(
                vrCamera.fieldOfView,
                zoomFOV,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            // Reset zoom
            vrCamera.fieldOfView = Mathf.Lerp(
                vrCamera.fieldOfView,
                originalFOV,
                Time.deltaTime * zoomSpeed
            );
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Mark object as grabbed
        if (!hasBeenGrabbed)
        {
            hasBeenGrabbed = true;
            Debug.Log(gameObject.name + " has been grabbed!");
        }

        isTriggered = true;
        targetObject = transform;

        ShowInfoText(gameObject.name);

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(true);

        if (infoPanelUI != null)
            infoPanelUI.SetActive(true);
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isTriggered = false;
        targetObject = null;

        if (typewriterEffect != null)
            typewriterEffect.Clear();

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(false);

        if (infoPanelUI != null)
            infoPanelUI.SetActive(false);

        // Trigger the question sequence
        if (questionToAnswerScript != null)
            questionToAnswerScript.BeginQuestion();
    }

    void LoadInfoText()
    {
        if (!File.Exists(infoTextFilePath))
        {
            Debug.LogError("File not found!");
            return;
        }

        string[] lines = File.ReadAllLines(infoTextFilePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(new char[] { '-' }, 2);

            if (parts.Length < 2)
                continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim().Trim('[', ']');

            infoDict[key] = value;

            Debug.Log("Loaded: " + key + " -> " + value);
        }
    }

    void ShowInfoText(string objectName)
    {
        if (typewriterEffect == null)
            return;

        if (infoDict.TryGetValue(objectName, out string info))
        {
            typewriterEffect.StartTypewriter(info);
            StopAllCoroutines(); // Stop any previous coroutines
            StartCoroutine(ShowInfoTextCoroutine(info));
        }
        else
        {
            typewriterEffect.Clear();
        }
    }

    System.Collections.IEnumerator ShowInfoTextCoroutine(string info)
    {
        string[] segments = info.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

        foreach (string segment in segments)
        {
            typewriterEffect.Clear();
            typewriterEffect.StartTypewriter(segment.Trim());
            // Wait for the typewriter to finish (if you have a way to detect this), or just wait a fixed time
            // Here, we wait 4 seconds after each segment
            yield return new WaitForSeconds(4f);
        }
    }
}