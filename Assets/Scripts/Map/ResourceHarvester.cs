using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceHarvester : Unit
{
	public Building resourceBase;
	public int harvestBonus = 0;
	public ResourceType resourceType;

	private Tile targetTile;
	private Tile viaTile;

	public HarvesterState currentState;
	private HarvesterState lastState;

	List<PathTile> path = new List<PathTile>();
	//LineRenderer lineRenderer;

	public float moveSpeed;

	public int currentResourceAmount;

	delegate void StateAction();

	private Dictionary<HarvesterState, StateAction> stateMethods;

	private int pathIndex;

	protected Animator animator;

	private const int ANIM_MOVE = 0;
	private const int ANIM_HARVEST = 1;
	private const int ANIM_DUMP = 2;

	public PathTile [] debugPath;

	public bool visible = true;

	public enum HarvesterState
	{
		Idle,
		Paused,
		SearchForResource,
		SearchForBuilding,
		Moving,
		Harvesting,
		Dumping,
		Finished,
		Stuck
	}

	// Use this for initialization
	void Start ()
	{

		stateMethods = new Dictionary<HarvesterState, StateAction>();
		stateMethods.Add (HarvesterState.Paused, Pause);
		stateMethods.Add (HarvesterState.Idle, Idle);
		stateMethods.Add (HarvesterState.SearchForResource, GetTargetResourceTile);
		stateMethods.Add (HarvesterState.SearchForBuilding, GetTargetBuildingTile);
		stateMethods.Add (HarvesterState.Moving, GoToTargetTile);
		stateMethods.Add (HarvesterState.Harvesting, Harvest);
		stateMethods.Add (HarvesterState.Dumping, AddResourceToBuilding);
		stateMethods.Add (HarvesterState.Finished, Idle);
		stateMethods.Add (HarvesterState.Stuck, Stuck);

		//lineRenderer = GetComponent<LineRenderer>();

		//animator = GetComponentInChildren<Animator>();

		SetTransparency(true);
		SetState(HarvesterState.Idle);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		if (built && currentState == HarvesterState.Idle) 
		{
			//play sound effect

			SetState(HarvesterState.SearchForResource);
		}

		//if the path is pulled out from under the harvester in the middle of the movement phase, i.e. a building is put on top of the harvester
		if (tileMap.GetPathTile(transform.localPosition) == null)
		{

			Debug.Log("PATH TILE IS NULL FOR HARVESTER");

			Tile currentTile = tileMap.GetTile(transform.localPosition);

			//if we're under a building
			if (currentTile.building)
			{
				//Debug.Log(currentTile.building);
				//Debug.Log(currentTile.building.GetNearestAdjacentTile(transform.position));
				transform.position = currentTile.building.GetNearestAdjacentTile(transform.position).transform.position;


				if (currentState == HarvesterState.Moving)
				{
					StopAllCoroutines();

					Debug.Log("update path");
					path.Clear();
					pathIndex = 0;
					targetTile.harvestersTargeting --;

					if (TileIsHarvestable(targetTile))
					{
						viaTile.harvestersTargeting --;
						SetState(HarvesterState.SearchForResource);
					}
					else
					{
						SetState(HarvesterState.SearchForBuilding);
					}
				}
			}
		}

		if (Map.instance.Pause && currentState != HarvesterState.Paused) //|| Map.instance.tileMap != tileMap) 
		{
			SetState(HarvesterState.Paused);
		}
		else if (!(Map.instance.Pause) && currentState == HarvesterState.Paused) // && Map.instance.tileMap == tileMap
		{
			Unpause();
		}
			
	}

	public void Pause()
	{
		StopAllCoroutines();
	}

	public void Unpause()
	{
		SetState(lastState);
	}

	public void Setup(Building building, TileMap theTileMap)
	{
		resourceBase = building;
		tileMap = theTileMap;
	}

	public void SetState(HarvesterState newState)
	{
		Debug.Log("SET STATE " + newState);

		lastState = currentState;
		currentState = newState;


		stateMethods[newState]();

	}

	public void Idle()
	{
		//at the moment, do nothing. ideally, an animation?
	}

	public void Stuck()
	{
		StartCoroutine(WaitAndRecheck());
	}

	IEnumerator WaitAndRecheck()
	{
		yield return new WaitForSeconds(2);

		SetState(lastState);
	}

	public void GetTargetResourceTile()
	{

		if (!visible) SetVisible(true);

		targetTile = GetClosestResourceTileOfType(resourceType, transform.position);

		if (targetTile != null) 
		{
			viaTile = targetTile.GetLeastTargetedAdjacentTile(transform.position);

			if (viaTile != null)
			{
				targetTile.harvestersTargeting ++;
				viaTile.harvestersTargeting ++;
				SetState(HarvesterState.Moving);
			}
			else
			{
				SetState(HarvesterState.Stuck);

			}

		}
		else
		{
			SetState(HarvesterState.Stuck);
		}
	}

	public void GetTargetBuildingTile()
	{
		targetTile = resourceBase.GetLeastTargetedAdjacentTile(transform.position);

		if (targetTile != null)
		{
			targetTile.harvestersTargeting ++;
			SetState(HarvesterState.Moving);
		}
	}

	public ResourceTile GetClosestResourceTileOfType(ResourceType typeToFind, Vector3 pos)
	{
		ResourceTile closestTile = null;
		
		float dist;
		float shortestDist = float.NaN;

		bool availableTilesBuiltOn = false;
		
		for (int i = 0; i < tileMap.resourceTiles.Count; i++)
		{
			/*Debug.Log("TILEMAP " + tileMap);
			Debug.Log("resourceTiles " + tileMap.resourceTiles);*/

			if (tileMap.resourceTiles[i] == null) continue;

			/*Debug.Log("resourceTiles i " + tileMap.resourceTiles[i], tileMap.resourceTiles[i]);
			Debug.Log("resourceTiles i type" + tileMap.resourceTiles[i].resourceType);
			Debug.Log("type " + typeToFind);*/
			//Debug.Log("TILEMAP");

			if (tileMap.resourceTiles[i].resourceType == typeToFind)
			{

				dist = Vector3.Distance(tileMap.resourceTiles[i].transform.position, pos);
				
				if (dist < shortestDist || float.IsNaN(shortestDist))
				{
					if (tileMap.resourceTiles[i].pathTile == null)
					{
						availableTilesBuiltOn = true;
					}
					else
					{
						closestTile = tileMap.resourceTiles[i];
						shortestDist = dist;
					}

					//Debug.Log ("Dist is less than shortest dist, tile is " + closestTile);
				}
			}
		}

		if (closestTile != null)
		{
			return closestTile;
		}
		else if (availableTilesBuiltOn)
		{
			return null;
		}
		else
		{

			SetState(HarvesterState.Finished);
			//Debug.LogError("No tile of type " + typeToFind + " found.");

			return null;
		}
	}


	public void GoToTargetTile()
	{
		//animator.SetInteger("Action", ANIM_MOVE);

		if (path.Count > 0)
		{
			StartCoroutine(WalkPath());
		}
		else if (TileIsHarvestable(targetTile))
		{
			if (targetTile != null && viaTile != null && tileMap.FindPathVia(transform.localPosition, viaTile.coords, targetTile.coords, path))
				//if (tileMap.FindPath(transform.localPosition, viaTile.coords, path))
			{
				/*lineRenderer.SetVertexCount(path.Count);
		for (int i = 0; i < path.Count; i++)
		lineRenderer.SetPosition(i, path[i].transform.position);*/
				
				StopAllCoroutines();
				StartCoroutine(WalkPath());
			}
			else
			{
				SetState(HarvesterState.Stuck);
			}


		}
		else
		{
			if (targetTile != null && tileMap.FindPath(transform.localPosition, targetTile.coords, path))
			{
				/*lineRenderer.SetVertexCount(path.Count);
			for (int i = 0; i < path.Count; i++)
				lineRenderer.SetPosition(i, path[i].transform.position);*/
				
				StopAllCoroutines();
				StartCoroutine(WalkPath());
			}
			else
			{
				SetState(HarvesterState.Stuck);
			}
		}


			
	}
	
	public void Harvest()
	{

		currentResourceAmount = (targetTile as ResourceTile).HarvestResources(harvestBonus);

		if (targetTile != null) targetTile.harvestersTargeting --;
		if (viaTile != null) viaTile.harvestersTargeting --;

		StartCoroutine(WaitForAnimation(ANIM_HARVEST, HarvesterState.SearchForBuilding));
	}

	public void ArrivedAtTargetTile()
	{
		Debug.Log("ARRIVED AT TARGET");

		if (TileIsHarvestable(targetTile))
		{
			SetState(HarvesterState.Harvesting);
		}
		else
		{
			SetState(HarvesterState.Dumping);
		}
	}

	public void AddResourceToBuilding()
	{

		if (visible) SetVisible(false);

		resourceBase.AddResourceFromHarvester(resourceType, currentResourceAmount);
		
		currentResourceAmount = 0;

		targetTile.harvestersTargeting --;

		StartCoroutine(WaitForAnimation(ANIM_DUMP, HarvesterState.SearchForResource));

		//SetState(HarvesterState.SearchForResource);

	}

	public void SetVisible(bool visibility)
	{

		Renderer[] rens = GetComponentsInChildren<MeshRenderer>();
		
		for (int i = 0; i < rens.Length; i++)
		{
			rens[i].enabled = visibility;
		}

		visible = visibility;
	}

	IEnumerator WaitForAnimation(int animState, HarvesterState nextState)
	{
		//animator.SetInteger("Action", animState);

		yield return new WaitForSeconds(2);

		SetState(nextState);
	}


	IEnumerator WalkPath()
	{
		while (pathIndex < path.Count)
		{

			if (path[pathIndex] != null)
			{
				transform.LookAt(path[pathIndex].transform.position, transform.parent.up);
				yield return StartCoroutine(WalkTo(path[pathIndex].transform.position));
				pathIndex++;
			}
			else
			{
				yield return 0;
				path.Clear();
				pathIndex = 0;
				SetState(HarvesterState.Moving);
			}

		}

		Debug.Log("end of path");

		path.Clear();
		pathIndex = 0;

		ArrivedAtTargetTile();
	}
	
	IEnumerator WalkTo(Vector3 position)
	{

		while (Vector3.Distance(transform.position, position) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);


			yield return 0;
		}
		transform.position = position;
	}

	public bool TileIsHarvestable(Tile t)
	{
		if (t is ResourceTile)
		{
			if ((t as ResourceTile).resourceType == resourceType)
			{
				return true;
			}
		}

		return false;
	}

	
	/*public bool CanBuild()
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
	}*/
}

