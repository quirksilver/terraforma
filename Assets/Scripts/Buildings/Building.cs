﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

    public Vector2 size;

	public BuildingFootprint footprint;

    public Building[] prerequisites;

    public string DisplayName;

    //Res
    public int buildCostWater;
    public int buildCostHeat;
    public int buildCostAir;
    public int buildCostFood;
    public int buildCostMetal;

    public int runningCostWater;
    public int runningCostHeat;
    public int runningCostAir;
    public int runningCostFood;
    public int runningCostMetal;

    public int produceWater;
    public int produceHeat;
    public int produceAir;
    public int produceFood;
    public int produceMetal;

    public bool buildingActive=true;

	protected virtual void Awake () {
	
		footprint = GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;
		
		Debug.Log(footprint);
	
	}

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update () {
	}

	protected void HideFootprint()
	{
		footprint.hide();
	}

    public bool CanBuild()
    {
        if (ResourceManager.instance.GetResource(ResourceManager.ResourceType.Water) < buildCostWater ||
            ResourceManager.instance.GetResource(ResourceManager.ResourceType.Metal) < buildCostMetal ||
            ResourceManager.instance.GetResource(ResourceManager.ResourceType.Heat) < buildCostHeat||
            ResourceManager.instance.GetResource(ResourceManager.ResourceType.Food) < buildCostFood ||
            ResourceManager.instance.GetResource(ResourceManager.ResourceType.Air) < buildCostAir)
        {
            return false;
        }

        return true;
    }

    public virtual void Tick()
    {
        if (buildingActive)
        {
            ResourceManager.instance.AddResource(produceWater, ResourceManager.ResourceType.Water);
            ResourceManager.instance.AddResource(produceHeat, ResourceManager.ResourceType.Heat);
            ResourceManager.instance.AddResource(produceAir, ResourceManager.ResourceType.Air);
            ResourceManager.instance.AddResource(produceFood, ResourceManager.ResourceType.Food);
            ResourceManager.instance.AddResource(produceMetal, ResourceManager.ResourceType.Metal);

            ResourceManager.instance.RemoveResource(runningCostWater, ResourceManager.ResourceType.Water);
            ResourceManager.instance.RemoveResource(runningCostHeat, ResourceManager.ResourceType.Heat);
            ResourceManager.instance.RemoveResource(runningCostAir, ResourceManager.ResourceType.Air);
            ResourceManager.instance.RemoveResource(runningCostFood, ResourceManager.ResourceType.Food);
            ResourceManager.instance.RemoveResource(runningCostMetal, ResourceManager.ResourceType.Metal);
        }
    }
}
