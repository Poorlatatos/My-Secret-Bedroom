using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrabObjectiveManager : MonoBehaviour
{
    public InteractionBehaviour[] interactionObjects;
    public ViewOnlyInteraction[] viewObjects;
    public GrabTracker[] objectsToGrab;

    private bool loadedScene = false;

    void Update()
    {
        if (loadedScene)
            return;

        // Check InteractionBehaviour objects
        foreach (InteractionBehaviour obj in interactionObjects)
        {
            if (!obj.hasBeenGrabbed)
                return;
        }

        // Check ViewOnlyInteraction objects
        foreach (ViewOnlyInteraction obj in viewObjects)
        {
            if (!obj.hasBeenViewed)
                return;
        }

        // Check GrabTracker objects
        foreach (GrabTracker obj in objectsToGrab)
        {
            if (!obj.hasBeenGrabbed)
                return;
        }

        loadedScene = true;

        Debug.Log("All objectives completed! Loading next scene in 60 seconds...");
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(30f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}