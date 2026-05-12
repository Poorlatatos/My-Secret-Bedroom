using UnityEngine;

public class HeadGestureDetector : MonoBehaviour
{
    public Transform xrCamera; // Assign in inspector or auto-find in Start()

    private float lastPitch;
    private float lastYaw;
    private float nodAccumulator = 0f;
    private float shakeAccumulator = 0f;
    private float gestureThreshold = 30f; // degrees
    private float gestureCooldown = 1f; // seconds
    private float lastGestureTime = -10f;

    void Start()
    {
        if (xrCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                xrCamera = mainCam.transform;
        }
        if (xrCamera != null)
        {
            lastPitch = xrCamera.eulerAngles.x;
            lastYaw = xrCamera.eulerAngles.y;
        }
    }

    void Update()
    {
        if (xrCamera == null)
            return;

        float pitch = xrCamera.eulerAngles.x;
        float yaw = xrCamera.eulerAngles.y;

        float pitchDelta = Mathf.DeltaAngle(lastPitch, pitch);
        float yawDelta = Mathf.DeltaAngle(lastYaw, yaw);

        // Accumulate pitch and yaw changes
        nodAccumulator += pitchDelta;
        shakeAccumulator += yawDelta;

        // Detect nod (yes)
        if (Mathf.Abs(nodAccumulator) > gestureThreshold && Time.time - lastGestureTime > gestureCooldown)
        {
            Debug.Log("Yes");
            lastGestureTime = Time.time;
            nodAccumulator = 0f;
            shakeAccumulator = 0f;
        }
        // Detect shake (no)
        else if (Mathf.Abs(shakeAccumulator) > gestureThreshold && Time.time - lastGestureTime > gestureCooldown)
        {
            Debug.Log("No");
            lastGestureTime = Time.time;
            nodAccumulator = 0f;
            shakeAccumulator = 0f;
        }

        lastPitch = pitch;
        lastYaw = yaw;
    }
}