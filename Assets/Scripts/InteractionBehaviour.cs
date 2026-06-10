using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Collections;

public class InteractionBehaviour : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera vrCamera;
    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;

    [Header("UI References")]
    public TMP_Text infoTextUI;
    public GameObject infoPanelUI;
    public GameObject blackScreenOverlay;

    [Header("Scripts")]
    public TypewriterEffect typewriterEffect;
    public QuestionToAnswerScript questionToAnswerScript;

    private float originalFOV;
    private bool isTriggered = false;
    private Transform targetObject;

    public bool hasBeenGrabbed = false;

    private DatabaseReference dbReference;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        originalFOV = vrCamera.fieldOfView;

        if (infoTextUI != null)
            infoTextUI.text = "";

        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnSelectEnter);
            simpleInteractable.selectExited.AddListener(OnSelectExit);
        }

        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Initialized");
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + task.Result);
            }
        });
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (!hasBeenGrabbed)
        {
            hasBeenGrabbed = true;
        }

        isTriggered = true;
        targetObject = transform;

        LoadDescription(gameObject.name);

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

    void LoadDescription(string itemName)
    {
        if (dbReference == null)
        {
            Debug.LogWarning("Firebase not ready yet.");
            return;
        }

        dbReference.Child("Info").Child(itemName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    string rawText = task.Result.Value.ToString();

                    // SAME LOGIC AS YOUR TXT FILE (<br> SPLIT)
                    string[] lines = rawText.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

                    if (typewriterEffect != null)
                        typewriterEffect.Clear();

                    StartCoroutine(DisplayLines(lines));
                }
                else
                {
                    Debug.LogWarning("No description found for: " + itemName);

                    if (infoTextUI != null)
                        infoTextUI.text = "No information available.";
                }
            });
    }

    IEnumerator DisplayLines(string[] lines)
    {
        foreach (string line in lines)
        {
            string cleanLine = line.Trim();

            if (string.IsNullOrEmpty(cleanLine))
                continue;

            if (typewriterEffect != null)
            {
                typewriterEffect.Clear();
                typewriterEffect.StartTypewriter(cleanLine);
            }
            else if (infoTextUI != null)
            {
                infoTextUI.text = cleanLine;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    void Update()
    {
        if (isTriggered && targetObject != null)
        {
            Vector3 direction =
                (targetObject.position - vrCamera.transform.position).normalized;

            Quaternion lookRotation =
                Quaternion.LookRotation(direction);

            vrCamera.transform.rotation =
                Quaternion.Slerp(
                    vrCamera.transform.rotation,
                    lookRotation,
                    Time.deltaTime * lockOnSpeed
                );

            vrCamera.fieldOfView =
                Mathf.Lerp(
                    vrCamera.fieldOfView,
                    zoomFOV,
                    Time.deltaTime * zoomSpeed
                );
        }
        else
        {
            vrCamera.fieldOfView =
                Mathf.Lerp(
                    vrCamera.fieldOfView,
                    originalFOV,
                    Time.deltaTime * zoomSpeed
                );
        }
    }

    private void OnDestroy()
    {
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnSelectEnter);
            simpleInteractable.selectExited.RemoveListener(OnSelectExit);
        }
    }
}