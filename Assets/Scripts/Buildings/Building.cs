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

	protected List<ResourceHarvester> harvesters = new List<ResourceHarvester>();

	protected List<Vector3> borderTiles;

	protected virtual void Awake () {
	
		footprint = GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;
		//Debug.Log(footprint);
	
	}

	// Use this for initialization
	public virtual void Start() {

	}
	
	// Update is called once per frame
	public virtual void Update () {

		Vector3 LBCorner = new Vector3(-0.5f, 0, -0.5f);
		Vector3 LTCorner = new Vector3(-0.5f, 0, 0.5f);
		Vector3 RTCorner = new Vector3(0.5f, 0, 0.5f);
		Vector3 RBCorner = new Vector3(0.5f, 0, -0.5f);
		
		
		for (int i = 0; i < borderTiles.Count; i++)
		{
			//Debug.Log("DRAW SOME LINES");
			Debug.DrawLine(transform.position + borderTiles[i] + LBCorner, transform.position + borderTiles[i] + LTCorner, Color.green);
			Debug.DrawLine(transform.position + borderTiles[i] + LTCorner, transform.position + borderTiles[i] + RTCorner, Color.green);
			Debug.DrawLine(transform.position + borderTiles[i] + RTCorner, transform.position + borderTiles[i] + RBCorner, Color.green);
			Debug.DrawLine(transform.position + borderTiles[i] + RBCorner, transform.position + borderTiles[i] + LBCorner, Color.green);
		}
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

	public void UpdateBorderTilePositions()
	{
		Debug.Log(borderTiles);

		for (int i  = borderTiles.Count - 1; i >= 0; i--)
		{
			Debug.Log("path tile " + Map.instance.tileMap.GetPathTile(transform.position + borderTiles[i]));
			if (Map.instance.tileMap.GetPathTile(transform.position + borderTiles[i]) == null)
			{
				borderTiles.RemoveAt(i);
			}
		}
	}

	public void GetBorderTilePositions()
	{
		Vector3 currentPos = Vector3.zero;

		borderTiles = new List<Vector3>();

		Vector3[] directions = new Vector3[4];
		directions[0] = Vector3.left;
		directions[1] = Vector3.forward;
		directions[2] = Vector3.right;
		directions[3] = Vector3.back;

		Vector3 checkPos;

		for (int i = 0; i < footprint.tilePositions.Count; i++)
		{
			for (int j = 0; j < directions.Length; j++)
			{
				checkPos = footprint.tilePositions[i]  + directions[j];

				if (footprint.tilePositions.IndexOf(checkPos) == -1 && borderTiles.IndexOf(checkPos) == -1)
				{
					borderTiles.Add(checkPos);
				}
			}
		}

		UpdateBorderTilePositions();

		//DEBUG
		/*
		Vector3 LBCorner = new Vector3(-0.5f, 0, -0.5f);
		Vector3 LTCorner = new Vector3(-0.5f, 0, 0.5f);
		Vector3 RTCorner = new Vector3(0.5f, 0, 0.5f);
		Vector3 RBCorner = new Vector3(0.5f, 0, -0.5f);
		
		
		for (int i = 0; i < borderTiles.Count; i++)
		{
			Debug.Log("DRAW SOME LINES");
			Debug.DrawLine(borderTiles[i] + LBCorner, borderTiles[i] + LTCorner, Color.green, 50.0f, true);
			Debug.DrawLine(borderTiles[i] + LTCorner, borderTiles[i] + RTCorner, Color.green, 50.0f, true);
			Debug.DrawLine(borderTiles[i] + RTCorner, borderTiles[i] + RBCorner, Color.green, 50.0f, true);
			Debug.DrawLine(borderTiles[i] + RBCorner, borderTiles[i] + LBCorner, Color.green, 50.0f, true);
		}*/

	}
}