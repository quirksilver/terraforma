using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceHarvester : MonoBehaviour
{
	public Building resourceBase;
	public int harvestBonus = 0;
	public ResourceManager.ResourceType resourceType;

	private Tile targetTile;

	private HarvesterState state;

	List<PathTile> path = new List<PathTile>();
	LineRenderer lineRenderer;

	public float moveSpeed;

	public int currentResourceAmount;

	delegate void StateAction();

	private Dictionary<HarvesterState, StateAction> stateMethods;

	private TileMap tileMap;

	public enum HarvesterState
	{
		Idle,
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

	}

	public void Setup(Building building)
	{
		resourceBase = building;
	}

	public void SetState(HarvesterState newState)
	{
		Debug.Log("SET STATE " + newState);
		stateMethods[newState]();

	}

	public void GetTargetResourceTile()
	{
		targetTile = GetClosestResourceTileOfType(resourceType, this.transform.position);
		SetState(HarvesterState.Moving);
	}

	public void GetTargetBuildingTile()
	{
		targetTile = resourceBase.GetNearestTile(transform.position);
		SetState(HarvesterState.Moving);
	}

	public ResourceTile GetClosestResourceTileOfType(ResourceManager.ResourceType typeToFind, Vector3 pos)
	{
		ResourceTile closestTile = null;
		
		float dist;
		float shortestDist = float.NaN;
		
		for (int i = 0; i < tileMap.resourceTiles.Count; i++)
		{
			if (tileMap.resourceTiles[i].resourceType == typeToFind)
			{
				dist = Vector3.Distance(tileMap.resourceTiles[i].coords, pos);
				
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
			Debug.LogError("No closest tile of type " + typeToFind + " found.");
			return null;
		}
	}


	public void GoToTargetTile()
	{
		if (tileMap.FindPath(transform.position, targetTile.coords, path))
		{
			lineRenderer.SetVertexCount(path.Count);
			for (int i = 0; i < path.Count; i++)
				lineRenderer.SetPosition(i, path[i].transform.position);
			
			StopAllCoroutines();
			StartCoroutine(WalkPath());
		}
	}
	
	public void Harvest()
	{
		currentResourceAmount = (targetTile as ResourceTile).HarvestResources(harvestBonus);

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

		SetState(HarvesterState.SearchForResource);

	}


	IEnumerator WalkPath()
	{
		var index = 0;
		while (index < path.Count)
		{
			yield return StartCoroutine(WalkTo(path[index].transform.position));
			index++;
		}

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
}
