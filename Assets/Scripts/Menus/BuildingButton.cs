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

        cost.AddResource(b.buildCostAir, ResourceType.Air);
        cost.AddResource(b.buildCostFood, ResourceType.Food);
        cost.AddResource(b.buildCostHeat, ResourceType.Heat);
        cost.AddResource(b.buildCostMetal, ResourceType.Metal);
        cost.AddResource(b.buildCostWater, ResourceType.Water);
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
            Map.instance.GetLevel().RemoveResource(building.buildCostAir, ResourceType.Air);
            Map.instance.GetLevel().RemoveResource(building.buildCostFood, ResourceType.Food);
            Map.instance.GetLevel().RemoveResource(building.buildCostHeat, ResourceType.Heat);
            Map.instance.GetLevel().RemoveResource(building.buildCostMetal, ResourceType.Metal);
            Map.instance.GetLevel().RemoveResource(building.buildCostWater, ResourceType.Water);

            menu.BuildBuilding(building.GetType());
        }
        cost.Tick();
    }
}
