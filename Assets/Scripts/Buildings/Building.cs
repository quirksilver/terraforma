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
		if (harvesterPrefab != null)
		{

			GameObject harvesterInstance = (GameObject)Instantiate(harvesterPrefab);
			ResourceHarvester newHarvester = harvesterInstance.GetComponent<ResourceHarvester>();
			newHarvester.transform.parent = transform.parent;
			newHarvester.transform.localPosition = transform.localPosition + GetRandomAdjacentTilePosition();
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
		return borderTiles[Mathf.RoundToInt(Random.Range(0, borderTiles.Count - 1))]; 
	}

	public Tile GetNearestAdjacentTile(Vector3 pos)
	{
		float dist;
		float shortestDist = float.NaN;
		Vector3 closestPos = Vector3.zero;
		
		for (int i = 0; i < borderTiles.Count; i++)
		{
				dist = Vector3.Distance(borderTiles[i]+transform.position, pos);
			
			if (dist < shortestDist || float.IsNaN(shortestDist))
			{
				shortestDist = dist;
					closestPos = borderTiles[i]+transform.position;
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