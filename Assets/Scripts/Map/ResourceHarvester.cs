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

	public HarvesterState currentState;
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

	protected Animator animator;

	private const int ANIM_MOVE = 0;
	private const int ANIM_HARVEST = 1;
	private const int ANIM_DUMP = 2;

	public PathTile [] debugPath;

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

		stateMethods = new Dictionary<HarvesterState, StateAction>();
		stateMethods.Add (HarvesterState.Paused, Pause);
		stateMethods.Add (HarvesterState.SearchForResource, GetTargetResourceTile);
		stateMethods.Add (HarvesterState.SearchForBuilding, GetTargetBuildingTile);
		stateMethods.Add (HarvesterState.Moving, GoToTargetTile);
		stateMethods.Add (HarvesterState.Harvesting, Harvest);
		stateMethods.Add (HarvesterState.Dumping, AddResourceToBuilding);

		lineRenderer = GetComponent<LineRenderer>();

		animator = GetComponentInChildren<Animator>();


		SetState(HarvesterState.SearchForResource);
	}

	// Update is called once per frame
	void Update ()
	{

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


				/*if (currentState == HarvesterState.Moving)
				{*/

				Debug.Log("update path");
				path.Clear();
				pathIndex = 0;
				targetTile.harvestersTargeting --;

				if (targetTile is ResourceTile)
				{
					viaTile.harvestersTargeting --;
					SetState(HarvesterState.SearchForResource);
				}
				else
				{
					SetState(HarvesterState.SearchForBuilding);
				}

					
					
				//}

			}
			else
			{
				Debug.Log ("shit be fucked");
			}

			//StopAllCoroutines();


			//jump to the nearest available border tile of the building on the current tile
			//hahah this is a terrible line of code, I'm not even going to apologise for it
			/*transform.position = tileMap.GetTile(transform.position).building.GetNearestAdjacentTile(transform.position).transform.position;
			path.Clear();
			pathIndex = 0;
			SetState(HarvesterState.Moving);*/
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
		//Debug.Log("SET STATE " + newState);

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

					//Debug.Log ("Dist is less than shortest dist, tile is " + closestTile);
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
		animator.SetInteger("Action", ANIM_MOVE);

		if (path.Count > 0)
		{
			StartCoroutine(WalkPath());
		}
		else if (targetTile is ResourceTile)
		{

			if (tileMap.FindPathVia(transform.localPosition, viaTile.coords, targetTile.coords, path))
			//if (tileMap.FindPath(transform.localPosition, viaTile.coords, path))
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

		StartCoroutine(WaitForAnimation(ANIM_HARVEST, HarvesterState.SearchForBuilding));
	}

	public void ArrivedAtTargetTile()
	{
		//Debug.Log("ARRIVED AT TARGET");

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

		StartCoroutine(WaitForAnimation(ANIM_DUMP, HarvesterState.SearchForResource));

		//SetState(HarvesterState.SearchForResource);

	}

	IEnumerator WaitForAnimation(int animState, HarvesterState nextState)
	{
		animator.SetInteger("Action", animState);

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
				path.Clear();
				pathIndex = 0;
				SetState(HarvesterState.Moving);
				yield break;
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

