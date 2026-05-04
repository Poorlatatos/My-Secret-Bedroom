using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class InteractionScript : MonoBehaviour
{
    public Camera vrCamera;
    public Transform targetObject;
    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;
    public TMP_Text infoTextUI;
    public string infoTextFilePath = "Assets/InfoText.txt";

    private bool isLockedOn = false;
    private float originalFOV;
    private Dictionary<string, string> infoDict = new Dictionary<string, string>();

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;
        originalFOV = vrCamera.fieldOfView;
        LoadInfoText();
        if (infoTextUI != null)
            infoTextUI.text = "";
    }

    void Update()
    {
        if (IsObjectInView())
        {
            if (!isLockedOn)
            {
                isLockedOn = true;
            }
            ShowInfoText(targetObject.name);
        }
        else
        {
            isLockedOn = false;
            if (infoTextUI != null)
                infoTextUI.text = "";
        }

        if (isLockedOn)
        {
            // Smoothly look at the target
            Vector3 direction = (targetObject.position - vrCamera.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            vrCamera.transform.rotation = Quaternion.Slerp(vrCamera.transform.rotation, lookRotation, Time.deltaTime * lockOnSpeed);

            // Smoothly zoom in
            vrCamera.fieldOfView = Mathf.Lerp(vrCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // Smoothly reset FOV
            vrCamera.fieldOfView = Mathf.Lerp(vrCamera.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
        }
    }

    bool IsObjectInView()
    {
        Vector3 viewportPoint = vrCamera.WorldToViewportPoint(targetObject.position);
        return viewportPoint.z > 0 &&
               viewportPoint.x > 0 && viewportPoint.x < 1 &&
               viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    void LoadInfoText()
    {
        if (!File.Exists(infoTextFilePath))
            return;

        foreach (var line in File.ReadAllLines(infoTextFilePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(new char[] { '-' }, 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim().Trim('[', ']');
                infoDict[key] = value;
            }
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