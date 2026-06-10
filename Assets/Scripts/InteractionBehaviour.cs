using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Networking;

public class InteractionBehaviour : MonoBehaviour
{
    public Camera vrCamera;
    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;

    public TMP_Text infoTextUI;
    public GameObject infoPanelUI;
    public GameObject blackScreenOverlay;

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

        // ✅ Use coroutine for Android/Quest
        StartCoroutine(LoadInfoText());

        if (infoTextUI != null)
            infoTextUI.text = "";

        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnSelectEnter);
        simpleInteractable.selectExited.AddListener(OnSelectExit);
    }

    // ✅ This replaces LoadInfoText() entirely
    IEnumerator LoadInfoText()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "InfoText.txt");

        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string[] lines = request.downloadHandler.text.Split('\n');

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
        else
        {
            Debug.LogError("Failed to load InfoText.txt: " + request.error);
        }
    }

    void Update()
    {
        if (isTriggered && targetObject != null)
        {
            Vector3 direction = (targetObject.position - vrCamera.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            vrCamera.transform.rotation = Quaternion.Slerp(
                vrCamera.transform.rotation,
                lookRotation,
                Time.deltaTime * lockOnSpeed
            );

            vrCamera.fieldOfView = Mathf.Lerp(
                vrCamera.fieldOfView,
                zoomFOV,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            vrCamera.fieldOfView = Mathf.Lerp(
                vrCamera.fieldOfView,
                originalFOV,
                Time.deltaTime * zoomSpeed
            );
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
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

        if (questionToAnswerScript != null)
            questionToAnswerScript.BeginQuestion();
    }

    void ShowInfoText(string objectName)
    {
        if (typewriterEffect == null)
            return;

        if (infoDict.TryGetValue(objectName, out string info))
        {
            StopAllCoroutines();
            StartCoroutine(ShowInfoTextCoroutine(info));
        }
        else
        {
            Debug.LogWarning("No info found for: " + objectName);
            typewriterEffect.Clear();
        }
    }

    IEnumerator ShowInfoTextCoroutine(string info)
    {
        string[] segments = info.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

        foreach (string segment in segments)
        {
            typewriterEffect.Clear();
            typewriterEffect.StartTypewriter(segment.Trim());
            yield return new WaitForSeconds(4f);
        }
    }
}