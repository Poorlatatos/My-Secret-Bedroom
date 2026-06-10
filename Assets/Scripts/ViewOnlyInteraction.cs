using UnityEngine;
using TMPro;
using System.Collections;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class ViewOnlyInteraction : MonoBehaviour
{
    [Header("Camera")]
    public Camera vrCamera;

    public float zoomFOV = 30f;
    public float zoomSpeed = 2f;
    public float lockOnSpeed = 2f;
    public float interactDistance = 10f;

    [Header("UI")]
    public TMP_Text infoTextUI;
    public GameObject infoPanelUI;
    public GameObject blackScreenOverlay;

    public TypewriterEffect typewriterEffect;

    private float originalFOV;
    private Transform targetObject;

    private bool isLookingAtObject = false;
    public bool hasBeenViewed = false;

    private DatabaseReference dbReference;
    private bool firebaseReady = false;

    private string pendingItem = "";
    private bool hasPendingRequest = false;
    private bool isFetching = false;
    private string currentItem = "";

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        originalFOV = vrCamera.fieldOfView;

        if (infoTextUI != null)
            infoTextUI.text = "";

        if (typewriterEffect != null && !typewriterEffect.gameObject.activeInHierarchy)
            typewriterEffect.gameObject.SetActive(true);

        InitializeFirebase();
    }


    void FetchFromFirebase(string itemName)
    {
        dbReference.Child("Info").Child(itemName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    string rawText = task.Result.Value.ToString();

                    string[] segments =
                        rawText.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

                    StopAllCoroutines();
                    StartCoroutine(ShowInfoTextCoroutine(segments));
                }
            });
    }
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                firebaseReady = true;

                Debug.Log("Firebase READY");

                if (hasPendingRequest)
                {
                    hasPendingRequest = false;
                    FetchFromFirebase(pendingItem);
                }
            }
            else
            {
                Debug.LogError("Firebase failed: " + status);
            }
        });
    }

    void Update()
    {
        Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform == transform)
            {
                if (!isLookingAtObject)
                {
                    isLookingAtObject = true;

                    if (!hasBeenViewed)
                    {
                        hasBeenViewed = true;
                        Debug.Log(gameObject.name + " viewed");
                    }

                    targetObject = transform;

                    LoadDescription(gameObject.name);

                    if (blackScreenOverlay != null)
                        blackScreenOverlay.SetActive(true);

                    if (infoPanelUI != null)
                        infoPanelUI.SetActive(true);
                }

                // lock rotation
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

                // zoom
                vrCamera.fieldOfView =
                    Mathf.Lerp(vrCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);

                return;
            }
        }

        // STOP LOOKING
        if (isLookingAtObject)
        {
            isLookingAtObject = false;
            targetObject = null;

            if (typewriterEffect != null)
                typewriterEffect.Clear();

            if (blackScreenOverlay != null)
                blackScreenOverlay.SetActive(false);

            if (infoPanelUI != null)
                infoPanelUI.SetActive(false);
        }

        // RESET ZOOM
        vrCamera.fieldOfView =
            Mathf.Lerp(vrCamera.fieldOfView, originalFOV, Time.deltaTime * zoomSpeed);
    }

    void LoadDescription(string itemName)
    {
        if (!firebaseReady)
        {
            Debug.LogWarning("Firebase not ready yet.");
            return;
        }

        // Prevent duplicate requests
        if (isFetching && currentItem == itemName)
            return;

        isFetching = true;
        currentItem = itemName;

        dbReference.Child("Info").Child(itemName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                isFetching = false;

                if (!task.IsCompletedSuccessfully || !task.Result.Exists)
                {
                    Debug.LogWarning("No data for: " + itemName);

                    if (infoTextUI != null)
                        infoTextUI.text = "No information available.";

                    return;
                }

                string rawText = task.Result.Value.ToString();

                string[] segments =
                    rawText.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

                StopAllCoroutines();
                StartCoroutine(ShowInfoTextCoroutine(segments));
            });
    }

    IEnumerator ShowInfoTextCoroutine(string[] segments)
    {
        foreach (string segment in segments)
        {
            string clean = segment.Trim();

            if (typewriterEffect != null)
            {
                typewriterEffect.Clear();
                typewriterEffect.StartTypewriter(clean);
            }
            else if (infoTextUI != null)
            {
                infoTextUI.text = clean;
            }

            yield return new WaitForSeconds(4f);
        }
    }
}