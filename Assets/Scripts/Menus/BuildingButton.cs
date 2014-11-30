using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour 
{
    private Building building;
    public Text label;
    private BuildMenu menu;
    public ResourceInfo cost;

    public void Setup(Building b, BuildMenu m)
    {
        building = b;
        label.text = b.name;
        menu = m;

        cost.AddResource(b.buildCostAir, ResourceManager.ResourceType.Air);
        cost.AddResource(b.buildCostFood, ResourceManager.ResourceType.Food);
        cost.AddResource(b.buildCostHeat, ResourceManager.ResourceType.Heat);
        cost.AddResource(b.buildCostMetal, ResourceManager.ResourceType.Metal);
        cost.AddResource(b.buildCostWater, ResourceManager.ResourceType.Water);

        cost.Tick();
    }

    public void Refresh()
    {
        bool valid = true;
        foreach (Building b in building.prerequisites)
        {
            if (Map.instance.GetBuildingsCount(b.GetType())==0)
            {
                valid = false;
            }
        }
        GetComponent<Button>().interactable = valid;
    }

    public void Build()
    {
        if (building.CanBuild())
        {
            ResourceManager.instance.RemoveResource(building.buildCostAir, ResourceManager.ResourceType.Air);
            ResourceManager.instance.RemoveResource(building.buildCostFood, ResourceManager.ResourceType.Food);
            ResourceManager.instance.RemoveResource(building.buildCostHeat, ResourceManager.ResourceType.Heat);
            ResourceManager.instance.RemoveResource(building.buildCostMetal, ResourceManager.ResourceType.Metal);
            ResourceManager.instance.RemoveResource(building.buildCostWater, ResourceManager.ResourceType.Water);

            menu.BuildBuilding(building.GetType());
        }
        cost.Tick();
    }
}
