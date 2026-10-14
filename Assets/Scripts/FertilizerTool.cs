using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FertilizerTool : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable grabInteractable;

    private void OnTriggerStay(Collider other)
    {
        if (grabInteractable.isSelected && other.TryGetComponent<FarmCell>(out var cell))
            cell.Fertilize();
    }
}
