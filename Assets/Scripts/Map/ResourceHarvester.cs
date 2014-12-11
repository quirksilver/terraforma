using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceHarvester : MonoBehaviour
{
	public Building resourceBase;
	public int harvestBonus = 0;
	public ResourceType resourceType;

	private Tile targetTile;
	private Tile viaTile;

	private HarvesterState currentState;
	private HarvesterState lastState;

	List<PathTile> path = new List<PathTile>();
	LineRenderer lineRenderer;

	public float moveSpeed;

	public int currentResourceAmount;

	delegate void StateAction();

	private Dictionary<HarvesterState, StateAction> stateMethods;

	private TileMap tileMap;

	private int pathIndex;

	public int buildCostWater;
	public int buildCostHeat;
	public int buildCostAir;
	public int buildCostFood;
	public int buildCostMetal;

	public enum HarvesterState
	{
		Idle,
		Paused,
		SearchForResource,
		SearchForBuilding,
		Moving,
		Harvesting,
		Dumping
	}

	// Use this for initialization
	void Start ()
	{
		tileMap = Map.instance.tileMap;

		stateMethods = new Dictionary<HarvesterState, StateAction>();
		stateMethods.Add (HarvesterState.Paused, Pause);
		stateMethods.Add (HarvesterState.SearchForResource, GetTargetResourceTile);
		stateMethods.Add (HarvesterState.SearchForBuilding, GetTargetBuildingTile);
		stateMethods.Add(HarvesterState.Moving, GoToTargetTile);
		stateMethods.Add(HarvesterState.Harvesting, Harvest);
		stateMethods.Add (HarvesterState.Dumping, AddResourceToBuilding);

		lineRenderer = GetComponent<LineRenderer>();

		SetState(HarvesterState.SearchForResource);

	}

	// Update is called once per frame
	void Update ()
	{
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

	public void Setup(Building building)
	{
		resourceBase = building;
	}

	public void SetState(HarvesterState newState)
	{
		Debug.Log("SET STATE " + newState);

		lastState = currentState;
		currentState = newState;


		stateMethods[newState]();

	}

	public void GetTargetResourceTile()
	{
		targetTile = GetClosestResourceTileOfType(resourceType, transform.position);
		viaTile = targetTile.GetLeastTargetedAdjacentTile(transform.position);
		targetTile.harvestersTargeting ++;
		viaTile.harvestersTargeting ++;
		SetState(HarvesterState.Moving);
	}

	public void GetTargetBuildingTile()
	{
		targetTile = resourceBase.GetLeastTargetedAdjacentTile(transform.position);
		targetTile.harvestersTargeting ++;
		SetState(HarvesterState.Moving);
	}

	public ResourceTile GetClosestResourceTileOfType(ResourceType typeToFind, Vector3 pos)
	{
		ResourceTile closestTile = null;
		
		float dist;
		float shortestDist = float.NaN;
		
		for (int i = 0; i < tileMap.resourceTiles.Count; i++)
		{
			if (tileMap.resourceTiles[i].resourceType == typeToFind)
			{
				dist = Vector3.Distance(tileMap.resourceTiles[i].transform.position, pos);
				
				if (dist < shortestDist || float.IsNaN(shortestDist))
				{
					closestTile = tileMap.resourceTiles[i];
					shortestDist = dist;

					Debug.Log ("Dist is less than shortest dist, tile is " + closestTile);
				}
			}
		}
		
		if (closestTile != null)
		{
			return closestTile;
		}
		else
		{
			Debug.LogError("No tile of type " + typeToFind + " found.");
			return null;
		}
	}


	public void GoToTargetTile()
	{
		if (path.Count > 0)
		{
			StartCoroutine(WalkPath());
		}
		else if (targetTile is ResourceTile)
		{

			if (tileMap.FindPathVia(transform.localPosition, viaTile.coords, targetTile.coords, path))
			    {
				lineRenderer.SetVertexCount(path.Count);
			for (int i = 0; i < path.Count; i++)
				lineRenderer.SetPosition(i, path[i].transform.position);
				
				StopAllCoroutines();
				StartCoroutine(WalkPath());
			}
		}
		else
		{
			if (tileMap.FindPath(transform.localPosition, targetTile.coords, path))
			{
				lineRenderer.SetVertexCount(path.Count);
			for (int i = 0; i < path.Count; i++)
				lineRenderer.SetPosition(i, path[i].transform.position);
				
				StopAllCoroutines();
				StartCoroutine(WalkPath());
			}
		}


			
	}
	
	public void Harvest()
	{
		currentResourceAmount = (targetTile as ResourceTile).HarvestResources(harvestBonus);

		targetTile.harvestersTargeting --;

		SetState(HarvesterState.SearchForBuilding);
	}

	public void ArrivedAtTargetTile()
	{
		Debug.Log("ARRIVED AT TARGET");

		if (targetTile is ResourceTile)
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

		resourceBase.AddResourceFromHarvester(resourceType, currentResourceAmount);
		
		currentResourceAmount = 0;

		targetTile.harvestersTargeting --;
		viaTile.harvestersTargeting --;

		SetState(HarvesterState.SearchForResource);

	}


	IEnumerator WalkPath()
	{
		while (pathIndex < path.Count)
		{
			yield return StartCoroutine(WalkTo(path[pathIndex].transform.position));
			pathIndex++;
		}

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
}

