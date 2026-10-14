using System;
using UnityEngine;

public class FarmResources : MonoBehaviour
{
    [SerializeField, Min(0)] private int crops;
    [SerializeField, Min(0)] private int seeds = 9;
    [SerializeField, Min(0)] private int coins = 5;
    [SerializeField, Min(0)] private int fertilizer;
    [SerializeField, Min(0)] private int stones;
    [SerializeField, Min(0)] private int sprinklerKit;
    [SerializeField, Min(0)] private int pickaxeKit;
    [SerializeField, Min(0)] private int tankKit;
    [SerializeField, Min(0)] private int soilKit;

    public int Crops => crops;
    public event Action Changed;

    public int GetAmount(FarmResource resource) => resource switch
    {
        FarmResource.Crops => crops,
        FarmResource.Seeds => seeds,
        FarmResource.Coins => coins,
        FarmResource.Fertilizer => fertilizer,
        FarmResource.Stones => stones,
        FarmResource.SprinklerKit => sprinklerKit,
        FarmResource.PickaxeKit => pickaxeKit,
        FarmResource.TankKit => tankKit,
        FarmResource.SoilKit => soilKit,
        _ => throw new ArgumentOutOfRangeException(nameof(resource))
    };

    public void AddCrop() => Add(FarmResource.Crops, 1);

    public void Add(FarmResource resource, int amount)
    {
        Change(resource, amount);
        Changed?.Invoke();
    }

    public bool TrySpend(FarmResource resource, int amount)
    {
        if (GetAmount(resource) < amount)
            return false;

        Change(resource, -amount);
        Changed?.Invoke();
        return true;
    }

    public bool TryTrade(FarmResource cost, int costAmount, FarmResource reward, int rewardAmount)
    {
        if (GetAmount(cost) < costAmount)
            return false;

        Change(cost, -costAmount);
        Change(reward, rewardAmount);
        Changed?.Invoke();
        return true;
    }

    private void Change(FarmResource resource, int amount)
    {
        switch (resource)
        {
            case FarmResource.Crops: crops += amount; break;
            case FarmResource.Seeds: seeds += amount; break;
            case FarmResource.Coins: coins += amount; break;
            case FarmResource.Fertilizer: fertilizer += amount; break;
            case FarmResource.Stones: stones += amount; break;
            case FarmResource.SprinklerKit: sprinklerKit += amount; break;
            case FarmResource.PickaxeKit: pickaxeKit += amount; break;
            case FarmResource.TankKit: tankKit += amount; break;
            case FarmResource.SoilKit: soilKit += amount; break;
            default: throw new ArgumentOutOfRangeException(nameof(resource));
        }
    }

    public bool TryBuyKit(int stoneCost, int coinCost, FarmResource kit)
    {
        if (stones < stoneCost || coins < coinCost)
            return false;

        stones -= stoneCost;
        coins -= coinCost;
        Change(kit, 1);
        Changed?.Invoke();
        return true;
    }
}
