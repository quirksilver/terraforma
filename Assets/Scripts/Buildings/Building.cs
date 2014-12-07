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

	protected List<Tile> borderTiles;

	protected TileMap tileMap;

	protected virtual void Awake () {
	
		footprint = GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;
		//Debug.Log(footprint);
	
	}

	public virtual void Setup(TileMap t)
	{
		tileMap = t;
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
			Debug.DrawLine(borderTiles[i].transform.position + LBCorner, borderTiles[i].transform.position + LTCorner, Color.green);
			Debug.DrawLine(borderTiles[i].transform.position + LTCorner, borderTiles[i].transform.position + RTCorner, Color.green);
			Debug.DrawLine(borderTiles[i].transform.position + RTCorner, borderTiles[i].transform.position + RBCorner, Color.green);
			Debug.DrawLine(borderTiles[i].transform.position + RBCorner, borderTiles[i].transform.position + LBCorner, Color.green);
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
		if (harvesterPrefab != null)
		{

			GameObject harvesterInstance = (GameObject)Instantiate(harvesterPrefab);
			ResourceHarvester newHarvester = harvesterInstance.GetComponent<ResourceHarvester>();
			newHarvester.transform.parent = transform.parent;
			newHarvester.transform.position = GetRandomAdjacentTilePosition();
			harvesters.Add(newHarvester);
			newHarvester.Setup(this);
		}
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
        if (buildingActive)
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

            hud.AddRes(produceWater, ResourceType.Water);
            hud.AddRes(produceHeat, ResourceType.Heat);
            hud.AddRes(produceAir, ResourceType.Air);
            hud.AddRes(produceFood, ResourceType.Food);
            hud.AddRes(produceMetal, ResourceType.Metal);
        }
    }

	public void AddResourceFromHarvester(ResourceType type, int amount)
	{
        Map.instance.GetLevel().AddResource(amount, type);
		hud.AddRes(amount, type);
	}

	public Vector3 GetRandomAdjacentTilePosition()
	{
		return borderTiles[Mathf.RoundToInt(Random.Range(0, borderTiles.Count - 1))].transform.position; 
	}

	public Tile GetLeastTargetedAdjacentTile(Vector3 pos)
	{
		int lowestTarget = -1;
		Tile checkTile;

		List<Tile> leastTargetedTiles = new List<Tile>();

		borderTiles.Sort(TileTargetComparison);

		for (int i = 0; i < borderTiles.Count; i++)
		{
			checkTile = borderTiles[i];

			if (i == 0)
				lowestTarget = checkTile.harvestersTargeting;

			if (checkTile.harvestersTargeting > lowestTarget)
				break;

			leastTargetedTiles.Add(checkTile);

		}

		Debug.Log(leastTargetedTiles);

		if (leastTargetedTiles.Count > 0)
		{
			return GetNearestAdjacentTile(pos, leastTargetedTiles);
		}
		else
		{
			return leastTargetedTiles[0];
		}
			
	}

	public int TileTargetComparison(Tile tileA, Tile tileB)
	{

		if (tileA.harvestersTargeting == tileB.harvestersTargeting)
		{
			return 0;
		}
		else if (tileA.harvestersTargeting < tileB.harvestersTargeting)
		{
			return -1;
		}
		else //must be greater
		{
			return 1;
		}
	}

	public Tile GetNearestAdjacentTile(Vector3 pos, List<Tile> tileList)
	{
		float dist;
		float shortestDist = float.NaN;
		Tile closestTile = null;
		
		for (int i = 0; i < tileList.Count; i++)
		{
			Debug.Log(tileList[i]);

			dist = Vector3.Distance(tileList[i].transform.position, pos);
			
			if (dist < shortestDist || float.IsNaN(shortestDist))
			{
				shortestDist = dist;
				closestTile = tileList[i];
			}
		}
		
		return closestTile;
	}

	public Tile GetNearestAdjacentTile(Vector3 pos)
	{
		float dist;
		float shortestDist = float.NaN;
		Vector3 closestPos = Vector3.zero;
		
		for (int i = 0; i < borderTiles.Count; i++)
		{
				dist = Vector3.Distance(borderTiles[i].transform.position, pos);
			
			if (dist < shortestDist || float.IsNaN(shortestDist))
			{
				shortestDist = dist;
					closestPos = borderTiles[i].transform.position;
			}
		}
		
		return tileMap.GetTile(closestPos);
	}

	public void UpdateBorderTilePositions()
	{
		Debug.Log(borderTiles);

		for (int i  = borderTiles.Count - 1; i >= 0; i--)
		{
			Debug.Log("path tile " + tileMap.GetPathTile(borderTiles[i].transform.position));
			if (tileMap.GetPathTile(borderTiles[i].transform.position) == null)
			{
				borderTiles.RemoveAt(i);
			}
		}
	}

	public void GetBorderTilePositions()
	{
		Vector3 currentPos = Vector3.zero;

		borderTiles = new List<Tile>();

		List<Vector3> borderTilePositions = new List<Vector3>();

		Vector3[] directions = new Vector3[8];
		directions[0] = Vector3.left;
		directions[1] = Vector3.forward;
		directions[2] = Vector3.right;
		directions[3] = Vector3.back;
		directions[4] = Vector3.left + Vector3.forward;
		directions[5] = Vector3.left + Vector3.back;
		directions[6] = Vector3.right + Vector3.forward;
		directions[7] = Vector3.right + Vector3.back;

		Vector3 checkPos;

		for (int i = 0; i < footprint.tilePositions.Count; i++)
		{
			for (int j = 0; j < directions.Length; j++)
			{
				checkPos = footprint.tilePositions[i]  + directions[j];

				if (footprint.tilePositions.IndexOf(checkPos) == -1 && borderTilePositions.IndexOf(checkPos) == -1)
				{
					Tile tileToAdd = tileMap.GetTile(transform.position + checkPos);

					if (tileToAdd != null)
					{
						borderTilePositions.Add(checkPos);
						borderTiles.Add(tileToAdd);
					}

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