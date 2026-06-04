using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    public Transform playerCamera;

    void LateUpdate()
    {
        if (playerCamera == null)
            return;

        // Make UI face the player
        Vector3 direction = playerCamera.position - transform.position;

        // Optional: lock vertical tilt (VR comfort)
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}