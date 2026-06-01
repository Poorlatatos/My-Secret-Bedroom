using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Calendar : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material[] materialSequence;
    [SerializeField] private ViewOnlyInteraction viewOnlyInteraction;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private int currentMaterialIndex;
    private bool hasActivatedViewOnlyInteraction;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    private void Start()
    {
        if (targetRenderer == null || materialSequence == null || materialSequence.Length == 0)
            return;

        currentMaterialIndex = 0;
        targetRenderer.material = materialSequence[0];
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (targetRenderer == null || materialSequence == null || materialSequence.Length == 0)
            return;

        if (currentMaterialIndex >= materialSequence.Length - 1)
        {
            ActivateViewOnlyInteraction();
            return;
        }

        currentMaterialIndex++;
        targetRenderer.material = materialSequence[currentMaterialIndex];

        if (currentMaterialIndex == materialSequence.Length - 1)
            ActivateViewOnlyInteraction();
    }

    private void ActivateViewOnlyInteraction()
    {
        if (hasActivatedViewOnlyInteraction)
            return;

        hasActivatedViewOnlyInteraction = true;

        if (viewOnlyInteraction != null)
            viewOnlyInteraction.enabled = true;
    }
}