using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClothingLaundry : MonoBehaviour
{
    [Header("References")]
    public HeadGestureDetector gestureDetector;
    public GameObject objectToHide;
    public Transform teleportTarget; // Assign the target position/rotation for the hidden object
    public GameObject objectToSpawnPrefab;
    public Transform rightControllerTransform; // Assign the right hand/controller transform in Inspector
    private GameObject spawnedObject;
    private bool hasSpawned = false;
    private bool canAcceptNod = false;

    public void EnableLaundryInteraction()
    {
        canAcceptNod = true;
    }
    void Update()
    {
        if (!canAcceptNod || hasSpawned || gestureDetector == null)
            return;

        if (gestureDetector.DidNod())
        {
            // Hide the original object
            if (objectToHide != null && teleportTarget != null)
            {
                objectToHide.transform.position = teleportTarget.position;
                objectToHide.transform.rotation = teleportTarget.rotation;
            }

            // Spawn the new object on the right controller
            if (objectToSpawnPrefab != null && rightControllerTransform != null)
            {
                spawnedObject = Instantiate(objectToSpawnPrefab, rightControllerTransform);
                spawnedObject.transform.localPosition = Vector3.zero;
                spawnedObject.transform.localRotation = Quaternion.identity;
                // Add the collision handler
                spawnedObject.AddComponent<LaundryItemCollisionHandler>();
            }

            hasSpawned = true; // Prevent repeat
            canAcceptNod = false; // Prevent further interactions
        }
    }
}