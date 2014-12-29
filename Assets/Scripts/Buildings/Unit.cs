using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour
{
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


	protected BuildingHUD hud;

	[HideInInspector]
	public string eventName;
	
	protected Shader diffuse;
	protected Shader transparentDiffuse;

	public float buildingTime;
	protected float buildingTimer;

	[HideInInspector]
	public bool built = false;

	[HideInInspector]
	public bool unitActive=true;
	[HideInInspector]
	public bool resourcesAvailable = true;

	protected TileMap tileMap;
	
	protected Material unitMat;


	protected virtual void Awake()
	{
		diffuse = Shader.Find("Diffuse");
		transparentDiffuse = Shader.Find("Transparent/Diffuse");

		eventName = "BUILT" + DisplayName.ToUpper();

		
		unitMat = GetComponentInChildren<MeshRenderer>().material;


	}

	
	// Update is called once per frame
	public virtual void Update () {
		
		if (!built)
		{
			buildingTimer += Time.deltaTime;
			
			float i = buildingTimer/buildingTime;
			
			Color color = Color.white;
			color.a = i + (0.2f * Mathf.Sin(Time.time*Mathf.PI*2));
			SetColour(color);
			
			if (buildingTimer > buildingTime)
			{
				SetTransparency(false);
				SetColour(Color.white);
				built=true;
				
				StoryEventManager.SendEvent(eventName);
			}
		}
		
	}

	public void SetTransparency(bool isTransparent)
	{
		if (isTransparent)
		{
			unitMat.shader = transparentDiffuse;
		}
		else
		{
			unitMat.shader = diffuse;
		}
	}
	
	public void SetColour(Color col)
	{
		unitMat.color = col;
	}
	
	public virtual void Setup(TileMap t)
	{
		tileMap = t;
	}

	
	public bool CanBuild()
	{
		if (Map.instance.GetLevel().GetResource(ResourceType.Water) < buildCostWater ||
		    Map.instance.GetLevel().GetResource(ResourceType.Metal) < buildCostMetal ||
		    Map.instance.GetLevel().GetResource(ResourceType.Heat) < buildCostHeat||
		    Map.instance.GetLevel().GetResource(ResourceType.Food) < buildCostFood ||
		    Map.instance.GetLevel().GetResource(ResourceType.Air) < buildCostAir)
		{
			return false;
		}
		
		return true;
	}
	
	public virtual void Tick()
	{
		if (unitActive && built)
		{
			resourcesAvailable = true;
			
			if (runningCostAir > Map.instance.GetLevel().GetResource(ResourceType.Air)
			    || runningCostFood > Map.instance.GetLevel().GetResource(ResourceType.Food)
			    || runningCostHeat > Map.instance.GetLevel().GetResource(ResourceType.Heat)
			    || runningCostMetal > Map.instance.GetLevel().GetResource(ResourceType.Metal)
			    || runningCostWater > Map.instance.GetLevel().GetResource(ResourceType.Water))
			{
				resourcesAvailable = false;
			}
			
			if (resourcesAvailable)
			{
				Map.instance.GetLevel().AddResource(produceWater, ResourceType.Water);
				Map.instance.GetLevel().AddResource(produceHeat, ResourceType.Heat);
				Map.instance.GetLevel().AddResource(produceAir, ResourceType.Air);
				Map.instance.GetLevel().AddResource(produceFood, ResourceType.Food);
				Map.instance.GetLevel().AddResource(produceMetal, ResourceType.Metal);
				
				Map.instance.GetLevel().RemoveResource(runningCostWater, ResourceType.Water);
				Map.instance.GetLevel().RemoveResource(runningCostHeat, ResourceType.Heat);
				Map.instance.GetLevel().RemoveResource(runningCostAir, ResourceType.Air);
				Map.instance.GetLevel().RemoveResource(runningCostFood, ResourceType.Food);
				Map.instance.GetLevel().RemoveResource(runningCostMetal, ResourceType.Metal);

				//Dictionary<ResourceType, int> dict = new Dictionary<ResourceType, int>();

				List<KeyValuePair<ResourceType, int>> myList = new List<KeyValuePair<ResourceType, int>>();

				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Water, produceWater - runningCostWater));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Heat, produceHeat - runningCostHeat));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Air, produceAir - runningCostAir));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Food, produceFood - runningCostFood));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Metal, produceMetal - runningCostMetal));

				
				myList.Sort((firstPair,nextPair) =>
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
				);

				foreach (KeyValuePair<ResourceType, int> pair in myList)
				{
					hud.AddRes(pair.Value, pair.Key);
				}
			}
		}
	}

	public void SetHUD(BuildingHUD h)
	{
		hud = h;
	}
	
	public void RemoveHud()
	{
		BuildingHUDControl.instance.removeHud (hud.gameObject);
	}

}

