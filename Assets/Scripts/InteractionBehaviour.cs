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
    public GameObject blackScreenOverlay;
    public string infoTextFilePath = "Assets/InfoText.txt";

    private float originalFOV;
    private bool isTriggered = false;
    private Transform targetObject;
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

        // Get simple interactable component
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // Subscribe to events
        simpleInteractable.selectEntered.AddListener(OnTriggerEnter);
        simpleInteractable.selectExited.AddListener(OnTriggerExit);
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

    void OnTriggerEnter(SelectEnterEventArgs args)
    {
        isTriggered = true;
        targetObject = transform;

        ShowInfoText(gameObject.name);

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(true);
    }

    void OnTriggerExit(SelectExitEventArgs args)
    {
        isTriggered = false;
        targetObject = null;

        if (infoTextUI != null)
            infoTextUI.text = "";

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(false);
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
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(new char[] { '-' }, 2);

            if (parts.Length < 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim().Trim('[', ']');

            infoDict[key] = value;

            Debug.Log("Loaded: " + key + " -> " + value);
        }
    }

    void ShowInfoText(string objectName)
    {
        if (infoTextUI == null) return;

        if (infoDict.TryGetValue(objectName, out string info))
        {
            infoTextUI.text = info;
        }
        else
        {
            infoTextUI.text = "";
        }
    }
}