using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CropHarvest : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private CropGrowth crop;
    private FarmCell cell;

    public void Initialize(FarmCell plantedCell) => cell = plantedCell;

    private void OnEnable() => interactable.selectEntered.AddListener(Harvest);
    private void OnDisable() => interactable.selectEntered.RemoveListener(Harvest);

    private void Harvest(SelectEnterEventArgs args) => cell.Harvest(crop);
}
