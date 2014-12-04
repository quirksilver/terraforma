using UnityEngine;
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

    private BuildingHUD hud;

	public GameObject harvesterPrefab;

	private List<ResourceHarvester> harvesters = new List<ResourceHarvester>();

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

    public void SetHUD(BuildingHUD h)
    {
        hud = h;
    }

	public void CreateNewHarvester()
	{
		GameObject harvesterInstance = (GameObject)Instantiate(harvesterPrefab);
		ResourceHarvester newHarvester = harvesterInstance.GetComponent<ResourceHarvester>();
		harvesters.Add(newHarvester);
		newHarvester.Setup(this);

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

            hud.AddRes(produceWater, ResourceManager.ResourceType.Water);
            hud.AddRes(produceHeat, ResourceManager.ResourceType.Heat);
            hud.AddRes(produceAir, ResourceManager.ResourceType.Air);
            hud.AddRes(produceFood, ResourceManager.ResourceType.Food);
            hud.AddRes(produceMetal, ResourceManager.ResourceType.Metal);
        }
    }

	public void AddResourceFromHarvester(ResourceManager.ResourceType type, int amount)
	{
		ResourceManager.instance.AddResource(amount, type);
		hud.AddRes(amount, type);
	}

	public Tile GetNearestTile(Vector3 pos)
	{
		float dist;
		float shortestDist = float.NaN;
		Vector3 closestPos = Vector3.zero;

		for (int i = 0; i < this.footprint.tilePositions.Count; i++)
		{
			dist = Vector3.Distance(footprint.tilePositions[i]+transform.position, pos);

			if (dist < shortestDist || float.IsNaN(shortestDist))
			{
				shortestDist = dist;
				closestPos = footprint.tilePositions[i]+transform.position;
			}
		}

		return Map.instance.tileMap.GetTile(closestPos);
	}
}
