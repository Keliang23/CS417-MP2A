using UnityEngine;

public class FarmCell : MonoBehaviour
{
    [SerializeField] private GameObject untilledSoil;
    [SerializeField] private GameObject tilledSoilPrefab;
    [SerializeField] private GameObject seedlingPrefab;
    [SerializeField] private FarmResources resources;
    [SerializeField] private GameObject sprinklerPrefab;
    [SerializeField] private FarmCell[] neighbors;
    [SerializeField] private ActionFeedback feedback;

    private GameObject tilledSoil;
    private GameObject seedling;
    private SoilMoisture soil;
    private Sprinkler sprinkler;
    private CropGrowth cropGrowth;
    private CropHealth cropHealth;

    public bool IsTilled => tilledSoil != null;
    public bool IsPlanted => seedling != null;
    public bool HasSprinkler => sprinkler != null;
    public float Moisture => IsTilled ? soil.Moisture : 0f;
    public CropGrowth Crop => IsPlanted ? cropGrowth : null;
    public CropHealth Health => IsPlanted ? cropHealth : null;
    public Sprinkler Irrigator => sprinkler;
    public bool IsSoilUpgraded => IsTilled && soil.IsUpgraded;

    public bool Till()
    {
        if (IsTilled)
            return false;

        tilledSoil = Instantiate(tilledSoilPrefab, transform, false);
        tilledSoil.name = "tilledSoil";
        soil = tilledSoil.GetComponent<SoilMoisture>();
        untilledSoil.SetActive(false);
        return true;
    }

    public bool Plant()
    {
        if (!IsTilled || IsPlanted || HasSprinkler || !resources.TrySpend(FarmResource.Seeds, 1))
            return false;

        seedling = Instantiate(seedlingPrefab, tilledSoil.transform, false);
        seedling.name = "seedling";
        cropGrowth = seedling.GetComponent<CropGrowth>();
        cropHealth = seedling.GetComponent<CropHealth>();
        cropGrowth.Initialize(soil);
        cropHealth.Initialize(this, soil);
        seedling.GetComponentInChildren<CropHarvest>(true).Initialize(this);
        return true;
    }

    public bool Harvest(CropGrowth crop)
    {
        if (!IsPlanted || crop.gameObject != seedling || !crop.IsMature)
            return false;

        Destroy(seedling);
        seedling = null;
        resources.AddCrop();
        return true;
    }

    public void Water(float amount)
    {
        if (HasSprinkler)
            sprinkler.AddWater(amount);
        else if (IsTilled)
            soil.AddWater(amount);
    }

    public bool BuildSprinkler()
    {
        if (!IsTilled || IsPlanted || HasSprinkler)
        {
            feedback.Failure("Use an empty tilled plot");
            return false;
        }
        if (!resources.TrySpend(FarmResource.SprinklerKit, 1))
        {
            feedback.Failure("Buy a sprinkler kit first");
            return false;
        }
        var placed = Instantiate(sprinklerPrefab, transform, false);
        placed.name = "sprinkler";
        sprinkler = placed.GetComponent<Sprinkler>();
        sprinkler.Initialize(resources, neighbors);
        feedback.Success("Sprinkler built - fill with water");
        return true;
    }

    public bool UpgradeSoil()
    {
        if (!IsTilled || HasSprinkler || soil.IsUpgraded)
        {
            feedback.Failure("Use a tilled, unupgraded crop plot");
            return false;
        }
        if (!resources.TrySpend(FarmResource.SoilKit, 1))
        {
            feedback.Failure("Buy a soil kit first");
            return false;
        }
        soil.Upgrade();
        feedback.Success("Soil upgraded: drying 2 -> 1 / sec");
        return true;
    }

    public void CropDied(CropHealth crop)
    {
        if (seedling != crop.gameObject)
            return;

        seedling.SetActive(false);
        Destroy(seedling);
        seedling = null;
    }

    public bool Fertilize()
    {
        if (!IsPlanted)
            return false;

        var crop = seedling.GetComponent<CropGrowth>();
        if (crop.IsMature || crop.IsFertilized || !resources.TrySpend(FarmResource.Fertilizer, 1))
            return false;

        crop.Fertilize();
        return true;
    }
}
