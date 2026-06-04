using UnityEngine;
using UnityEngine.SceneManagement;

public class GrabObjectiveManager : MonoBehaviour
{
    public InteractionBehaviour[] objectsToCheck;

    private bool loadedScene = false;

    void Update()
    {
        if (loadedScene)
            return;

        foreach (InteractionBehaviour obj in objectsToCheck)
        {
            if (!obj.hasBeenGrabbed)
                return;
        }

        loadedScene = true;

        Debug.Log("All objects have been grabbed!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}