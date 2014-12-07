using UnityEngine;
using System.Collections;

public class BuildHarvesterButton : MonoBehaviour
{
	private ResourceHarvester harvester;
	public BuildingControl menu;
	public ResourceInfo cost;
	
	public void Setup(ResourceHarvester h, BuildingControl m)
	{
		harvester = h;
		menu = m;

		cost.ClearResources();

		cost.AddResource(h.buildCostAir, ResourceType.Air);
		cost.AddResource(h.buildCostFood, ResourceType.Food);
		cost.AddResource(h.buildCostHeat, ResourceType.Heat);
		cost.AddResource(h.buildCostMetal, ResourceType.Metal);
		cost.AddResource(h.buildCostWater, ResourceType.Water);
	}
	
	public void Refresh()
	{

	}
	
	public void Build()
	{
		if (Map.instance.Pause)
		{
			return;
		}
		
		if (harvester.CanBuild())
		{
			Map.instance.GetLevel().RemoveResource(harvester.buildCostAir, ResourceType.Air);
			Map.instance.GetLevel().RemoveResource(harvester.buildCostFood, ResourceType.Food);
			Map.instance.GetLevel().RemoveResource(harvester.buildCostHeat, ResourceType.Heat);
			Map.instance.GetLevel().RemoveResource(harvester.buildCostMetal, ResourceType.Metal);
			Map.instance.GetLevel().RemoveResource(harvester.buildCostWater, ResourceType.Water);
			
			menu.BuildHarvesterButton();
		}
		cost.Tick();
	}
}

