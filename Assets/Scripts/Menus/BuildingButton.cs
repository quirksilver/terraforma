﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour 
{
    public Building building;
    public Text label;
    private BuildMenu menu;
    public ResourceInfo cost;
	public ResourceInfo produces;
	public ResourceInfo requires;
	public Text prerequisitesLabel;
	public Image requiredStar;

	private bool prereqsMet=false;

    public void Setup(Building b, BuildMenu m)
    {
        building = b;
        label.text = b.DisplayName;
        menu = m;

        cost.AddResource(b.buildCostAir, ResourceType.Air);
        cost.AddResource(b.buildCostFood, ResourceType.Food);
        cost.AddResource(b.buildCostHeat, ResourceType.Heat);
        cost.AddResource(b.buildCostMetal, ResourceType.Metal);
        cost.AddResource(b.buildCostWater, ResourceType.Water);

		requires.AddResource (b.runningCostAir, ResourceType.Air);
		requires.AddResource (b.runningCostFood, ResourceType.Food);
		requires.AddResource (b.runningCostHeat, ResourceType.Heat);
		requires.AddResource (b.runningCostMetal, ResourceType.Metal);
		requires.AddResource (b.runningCostWater, ResourceType.Water);

		produces.AddResource(b.produceAir, ResourceType.Air);
		produces.AddResource(b.produceFood, ResourceType.Food);
		produces.AddResource(b.produceHeat, ResourceType.Heat);
		produces.AddResource(b.produceMetal, ResourceType.Metal);
		produces.AddResource(b.produceWater, ResourceType.Water);
    }

    public void Refresh()
    {
		prereqsMet = true;
		prerequisitesLabel.transform.parent.gameObject.SetActive (true);
		int lines = 0;
		prerequisitesLabel.text = "";

        foreach (Building b in building.prerequisites)
        {
            if (Map.instance.GetBuildingsCount(b.GetType())==0)
            {
				prereqsMet = false;
				lines++;
				if(lines<=2)
				{
					prerequisitesLabel.text += b.DisplayName + "\n";
				}
				else if(lines==3)
				{
					prerequisitesLabel.text += "etc.";
				}
            }
        }

		if (lines == 0) 
		{
			prerequisitesLabel.transform.parent.gameObject.SetActive (false);
		}
		GetComponent<Image> ().color = prereqsMet ? Color.white : new Color (255, 255, 255, 0.5f);
		cost.Tick ();
        //GetComponent<Button>().interactable = valid;
    }

    public void Build()
    {
        if (Map.instance.Pause)
        {
            return;
        }


		if (!prereqsMet)
		{
				menu.BuildBuilding(building.GetType(),true);
		}							
		else if (building.CanBuild() && prereqsMet)
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
