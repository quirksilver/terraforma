﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

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

	public ResourceType requiredResourceTileType = ResourceType.Count;
	public int numberResourceTilesRequired = 0;

    public float buildingTime;
    private float buildingTimer;
    private bool built = false;

    public bool buildingActive=true;

    private BuildingHUD hud;

	public GameObject harvesterPrefab;

	protected List<ResourceHarvester> harvesters = new List<ResourceHarvester>();

	protected List<Tile> borderTiles;

	protected TileMap tileMap;

	protected GameObject spriteHolder;
	protected GameObject fullSprite;

	protected int DebugSpawnTile = 0;

	protected virtual void Awake () {
	
		footprint = GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;
		//Debug.Log(footprint);

		Transform holderTransform = transform.Find("SpriteHolder");

		if (holderTransform) spriteHolder = holderTransform.gameObject;
		fullSprite = transform.Find("Sprite").gameObject;
	
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

        if (!built)
        {
            buildingTimer += Time.deltaTime;

            float i = buildingTimer/buildingTime;

            Color color = Color.white;
            color.a = i + (0.2f * Mathf.Sin(Time.time*Mathf.PI*2));
            GetComponentInChildren<SpriteRenderer>().color = color;

            if (buildingTimer > buildingTime)
            {
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
                built=true;
				if (spriteHolder)
				{
					spriteHolder.SetActive(true);
					fullSprite.SetActive(false);
				}
                StoryEventManager.SendEvent("BUILT" + DisplayName.ToUpper());
            }
        }

		/*Vector3 LBCorner = new Vector3(-0.5f, 0, -0.5f);
		Vector3 LTCorner = new Vector3(-0.5f, 0, 0.5f);
		Vector3 RTCorner = new Vector3(0.5f, 0, 0.5f);
		Vector3 RBCorner = new Vector3(0.5f, 0, -0.5f);
		
		
		for (int i = 0; i < borderTiles.Count; i++)
		{
			//Debug.Log("DRAW SOME LINES");
			Debug.DrawLine(borderTiles[i].transform.position + LBCorner, borderTiles[i].transform.position + LTCorner, Color.yellow);
			Debug.DrawLine(borderTiles[i].transform.position + LTCorner, borderTiles[i].transform.position + RTCorner, Color.yellow);
			Debug.DrawLine(borderTiles[i].transform.position + RTCorner, borderTiles[i].transform.position + RBCorner, Color.yellow);
			Debug.DrawLine(borderTiles[i].transform.position + RBCorner, borderTiles[i].transform.position + LBCorner, Color.yellow);
		}*/
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
			newHarvester.transform.localRotation = Quaternion.identity;
			harvesters.Add(newHarvester);
			newHarvester.Setup(this, tileMap);
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
        if (buildingActive && built)
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
		//debugging

		DebugSpawnTile ++;

		return borderTiles[DebugSpawnTile % borderTiles.Count].transform.position;
		//return borderTiles[Mathf.RoundToInt(Random.Range(0, borderTiles.Count - 1))].transform.position; 
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
//			Debug.Log(tileList[i]);

			dist = Vector3.Distance(tileList[i].transform.position, pos);
			
			if (dist < shortestDist || float.IsNaN(shortestDist))
			{
				shortestDist = dist;
				closestTile = tileList[i];
			}
		}


		/*Vector3 LBCorner = new Vector3(-0.5f, 0, -0.5f);
		Vector3 LTCorner = new Vector3(-0.5f, 0, 0.5f);
		Vector3 RTCorner = new Vector3(0.5f, 0, 0.5f);
		Vector3 RBCorner = new Vector3(0.5f, 0, -0.5f);
		Debug.Log(closestTile);

		Debug.DrawLine(closestTile.transform.position + LBCorner, closestTile.transform.position + LTCorner, Color.yellow, 20.0f);
		Debug.DrawLine(closestTile.transform.position + LTCorner, closestTile.transform.position + RTCorner, Color.yellow, 20.0f);
		Debug.DrawLine(closestTile.transform.position + RTCorner, closestTile.transform.position + RBCorner, Color.yellow, 20.0f);
		Debug.DrawLine(closestTile.transform.position + RBCorner, closestTile.transform.position + LBCorner, Color.yellow, 20.0f);*/
		
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
				closestPos = borderTiles[i].transform.localPosition;
			}
		}
		
		return tileMap.GetTile(closestPos);
	}

	public void UpdateBorderTilePositions()
	{
		Debug.Log(borderTiles);

		for (int i  = borderTiles.Count - 1; i >= 0; i--)
		{
			Debug.Log("path tile " + tileMap.GetPathTile(borderTiles[i].transform.localPosition));
			if (borderTiles[i].pathTile == null || borderTiles[i].pathTile.flaggedForDestruction == true)
			{
				Debug.Log("remove tile " + i);
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
		/*directions[4] = Vector3.left + Vector3.forward;
		directions[5] = Vector3.left + Vector3.back;
		directions[6] = Vector3.right + Vector3.forward;
		directions[7] = Vector3.right + Vector3.back;*/

		Vector3 checkPos;

		for (int i = 0; i < footprint.tilePositions.Count; i++)
		{
			for (int j = 0; j < directions.Length; j++)
			{
				checkPos = footprint.tilePositions[i]  + directions[j];

				if (footprint.tilePositions.IndexOf(checkPos) == -1 && borderTilePositions.IndexOf(checkPos) == -1)
				{
					Tile tileToAdd = tileMap.GetTile(transform.localPosition + checkPos);

					Debug.Log ("BORDER TILE " + tileToAdd);

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

	public void CreateSplitSprite()
	{
		GameObject clone = PrefabUtility.InstantiatePrefab(this.gameObject) as GameObject;

		string path = "Buildings/" + gameObject.name; //"Assets/tests/" + gameObject.name; //+ "/" + gameObject.name + "SpriteSheet.png"; //AssetDatabase.GetAssetPath(sprite.texture);
		//"Assets/Resources/Buildings/Dome/DomeSpriteSheet.png"

		//if (clone.

		GameObject spriteHolder = clone.transform.FindChild("SpriteHolder").gameObject;

		if (spriteHolder == null) 
		{
			spriteHolder = new GameObject("SpriteHolder");
		}
		else
		{

			var children = new List<GameObject>();
			foreach (Transform child in spriteHolder.transform) children.Add(child.gameObject);

			children.ForEach(child => DestroyImmediate(child));
		}

		spriteHolder.transform.position = Vector3.zero;


		Debug.Log(spriteHolder);
		Debug.Log(clone);

		spriteHolder.transform.parent = clone.transform;
		spriteHolder.transform.localPosition = Vector3.zero;


		Sprite tempSprite;
		GameObject tempObj;
		SpriteRenderer tempRenderer;

		//Texture2D texture = Resources.LoadAssetAtPath<Texture>(path) as Texture2D;

		//Debug.Log(Resources.LoadAssetAtPath<Texture>("Assets/tests/Dome/DomeSpriteSheet.png"));

		/*Debug.Log ("texture at path " + path);
		Debug.Log(texture);
		Debug.Log(texture.height);*/

		BuildingFootprint cloneFootprint = clone.GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;

		Debug.Log (cloneFootprint);
		cloneFootprint.CalculatePivot(false);

		Sprite[] sprites = Resources.LoadAll<Sprite>(path);

		Debug.Log(sprites.Length);


		for (int i = 0; i < cloneFootprint.tilePositions.Count; i++)
		{
//			tempSprite = Sprite.Create(texture, new Rect(i*135, 0, 134, texture.height), new Vector2(0.5f, 0.0f), 95);

			tempObj = new GameObject("sprite" + i);

			tempRenderer = tempObj.AddComponent<SpriteRenderer>() as SpriteRenderer;

			tempRenderer.sprite = sprites[i];
				//tempSprite;//;

			tempObj.AddComponent<BillboardSprite>();

			tempObj.transform.parent = spriteHolder.transform;

			tempObj.transform.localPosition = cloneFootprint.tilePositions[i] + new Vector3(-0.5f, 0.0f, -0.5f);
			                         
		}

		PrefabUtility.ReplacePrefab(clone, PrefabUtility.GetPrefabParent(clone), ReplacePrefabOptions.ConnectToPrefab);	

		DestroyImmediate(clone);
	}
}