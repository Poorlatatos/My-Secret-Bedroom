using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class ViewOnlyInteraction : MonoBehaviour
{
    public Camera vrCamera;

    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;
    public float interactDistance = 10f;

    public TMP_Text infoTextUI;

    // UI PANEL IMAGE
    public GameObject infoPanelUI;

    public GameObject blackScreenOverlay;

    public string infoTextFilePath = "Assets/InfoText.txt";

    public TypewriterEffect typewriterEffect;

    private float originalFOV;

    private Transform targetObject;

    private Dictionary<string, string> infoDict = new Dictionary<string, string>();

    private bool isLookingAtObject = false;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        originalFOV = vrCamera.fieldOfView;

        LoadInfoText();

        if (infoTextUI != null)
            infoTextUI.text = "";

        // Hide panel at start
        if (infoPanelUI != null)
            infoPanelUI.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(
            vrCamera.transform.position,
            vrCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if player is looking at this object
            if (hit.transform == transform)
            {
                if (!isLookingAtObject)
                {
                    isLookingAtObject = true;

                    targetObject = transform;

                    ShowInfoText(gameObject.name);

                    // Show black screen
                    if (blackScreenOverlay != null)
                        blackScreenOverlay.SetActive(true);

                    // Show UI panel
                    if (infoPanelUI != null)
                        infoPanelUI.SetActive(true);
                }

                // Lock camera onto object
                Vector3 direction =
                    (targetObject.position - vrCamera.transform.position).normalized;

                Quaternion lookRotation =
                    Quaternion.LookRotation(direction);

                vrCamera.transform.rotation = Quaternion.Slerp(
                    vrCamera.transform.rotation,
                    lookRotation,
                    Time.deltaTime * lockOnSpeed
                );

                // Zoom camera
                vrCamera.fieldOfView = Mathf.Lerp(
                    vrCamera.fieldOfView,
                    zoomFOV,
                    Time.deltaTime * zoomSpeed
                );

                return;
            }
        }

        // If player stopped looking
        if (isLookingAtObject)
        {
            isLookingAtObject = false;

            targetObject = null;

            if (typewriterEffect != null)
                typewriterEffect.Clear();

            // Hide black screen
            if (blackScreenOverlay != null)
                blackScreenOverlay.SetActive(false);

            // Hide UI panel
            if (infoPanelUI != null)
                infoPanelUI.SetActive(false);
        }

        // Reset zoom
        vrCamera.fieldOfView = Mathf.Lerp(
            vrCamera.fieldOfView,
            originalFOV,
            Time.deltaTime * zoomSpeed
        );
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

            string value = parts[1]
                .Trim()
                .Trim('[', ']');

            infoDict[key] = value;
        }
    }

    void ShowInfoText(string objectName)
    {
        if (typewriterEffect == null)
            return;

        if (infoDict.TryGetValue(objectName, out string info))
        {
            typewriterEffect.StartTypewriter(info);
        }
        else
        {
            typewriterEffect.Clear();
        }
    }
}