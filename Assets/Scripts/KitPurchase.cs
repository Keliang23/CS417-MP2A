using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KitPurchase : MonoBehaviour
{
    [SerializeField] private FarmResources resources;
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private ActionFeedback feedback;
    [SerializeField] private FarmResource kit;
    [SerializeField, Min(1)] private int stoneCost;
    [SerializeField, Min(1)] private int coinCost;

    private void OnEnable() => interactable.selectEntered.AddListener(Buy);
    private void OnDisable() => interactable.selectEntered.RemoveListener(Buy);
    private void Buy(SelectEnterEventArgs args)
    {
        if (resources.TryBuyKit(stoneCost, coinCost, kit))
            feedback.Success("Kit bought - take its carrier below");
        else
            feedback.Failure($"Need {stoneCost} stones + {coinCost} coins");
    }
}
