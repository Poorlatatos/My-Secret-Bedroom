
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
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

    private XRSimpleInteractable simpleInteractable;
    private Coroutine displayCoroutine;

    // Object descriptions
    private string GetDescription(string itemName)
    {
        switch (itemName)
        {
            case "Laptop":
                return "Looking through someone's laptop might be an invasion of privacy and erodes trust. Consider reaching out to them and sharing your concerns instead.";

            case "Speaker":
                return "Listening to emo songs does not necessarily mean that a young person has mental health issues.";

            case "Tablet":
                return "Young people experiencing mental health distress might have difficulties completing their work due to a drop in motivation levels and energy levels. <br>Instead of criticising them for unfinished work, approach them with concern.";

            case "NoticeBoard":
                return "Take interest in what the young person is interested in. It is important to build a connection with the young person, and to see them beyond their mental health struggles.";

            case "WallPoster":
                return "Take interest in what the young person is interested in. It is important to build a connection with the young person, and to see them beyond their mental health struggles.";

            case "Calendar":
                return "For some, a calendar serves as a reminder of the tasks and events they need to attend to. However, for a young person struggling with their mental health, attending social events, school/work can be difficult.";

            case "Mirror":
                return "When a person is struggling with their body image, they might experience significant distress having to look at their own physical appearance.";

            case "Phone":
                return "To outsiders, the missed messages and calls, and increased sleep might look like the young person is \"lazy\" or is \"anti-social\".<br>However, it's important to look beneath the surface.<br>When young people are experiencing mental health struggles, they might not have the physical and emotional energy to stay connected with others and might cope with their distress by sleeping more and rejecting social activities.";

            case "WastepaperBin":
                return "When a young person is struggling with feelings of low mood, they might experience changes in their appetite (e.g. eating more/less than usual). For someone who struggles with body image issues, they might throw away food that they like to restrict their calories.";

            case "WeighingScale":
                return "When a young person is struggling with their body image, they might become obsessed with their weight and how they look. This can sometimes lead to frequent monitoring of weight.";

            case "PileOfClothes":
                return "A young person experiencing mental health distress might be so affected by their low mood and low energy levels that they have difficulty keeping their room organised or doing household chores. <br>Small gestures of understanding—like asking how they're doing or offering practical help—can provide valuable support.";

            default:
                return "No information available.";
        }
    }

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        if (vrCamera != null)
            originalFOV = vrCamera.fieldOfView;

        if (infoTextUI != null)
            infoTextUI.text = "";

        if (infoPanelUI != null)
            infoPanelUI.SetActive(false);

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(false);

        simpleInteractable = GetComponent<XRSimpleInteractable>();

        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnSelectEnter);
            simpleInteractable.selectExited.AddListener(OnSelectExit);
        }
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        hasBeenGrabbed = true;
        isTriggered = true;
        targetObject = transform;

        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        if (infoPanelUI != null)
            infoPanelUI.SetActive(true);

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(true);

        string description = GetDescription(gameObject.name);

        if (description == "No information available.")
        {
            Debug.LogWarning("No description found for: " + gameObject.name);
        }

        string[] lines = description.Split(
            new string[] { "<br>" },
            System.StringSplitOptions.None
        );

        if (typewriterEffect != null)
            typewriterEffect.Clear();

        displayCoroutine = StartCoroutine(DisplayLines(lines));
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isTriggered = false;
        targetObject = null;

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        if (typewriterEffect != null)
            typewriterEffect.Clear();

        if (infoTextUI != null)
            infoTextUI.text = "";

        if (blackScreenOverlay != null)
            blackScreenOverlay.SetActive(false);

        if (infoPanelUI != null)
            infoPanelUI.SetActive(false);

        if (questionToAnswerScript != null)
            questionToAnswerScript.BeginQuestion();
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

        displayCoroutine = null;
    }

    void Update()
    {
        if (vrCamera == null)
            return;

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
