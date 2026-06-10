using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    public Transform playerCamera;

    void LateUpdate()
    {
        if (playerCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(
            transform.position - playerCamera.position
        );

        // Flip it around so the front faces the camera
        transform.Rotate(0f, 180f, 0f);
    }
}