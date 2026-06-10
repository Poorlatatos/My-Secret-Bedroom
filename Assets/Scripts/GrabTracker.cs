using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabTracker : MonoBehaviour
{
    public bool hasBeenGrabbed = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable baseInteractable;

    private void Awake()
    {
        // Try grab interactable first
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelected);
        }
        else
        {
            // fallback to any interactable (button, simple interact, etc.)
            baseInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            if (baseInteractable != null)
            {
                baseInteractable.selectEntered.AddListener(OnSelected);
            }
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        hasBeenGrabbed = true;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnSelected);

        if (baseInteractable != null)
            baseInteractable.selectEntered.RemoveListener(OnSelected);
    }
}