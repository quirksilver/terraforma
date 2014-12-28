using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public Vector3 coords;
    public Building building { private set; get; }
	public int harvestersTargeting = 0;

	public Building[] buildingRestrictions;

	public PathTile pathTile;

	// Use this for initialization
	public virtual void Setup () {
        building = null;

		coords = transform.localPosition;

		pathTile = GetComponent<PathTile>();
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

    public void AssignBuilding(Building b)
    {
        building = b;

		//(GetComponentInChildren<Renderer>() as Renderer).material.color = Color.blue;
	
		if (pathTile) pathTile.flaggedForDestruction = true;
    }

	public virtual void OnMouseEnter()
    {
		//Debug.Log("MOUSE OVER TILE  " + coords);
        Map.instance.MouseOver(coords);
    }

	public Tile GetLeastTargetedAdjacentTile(Vector3 pos)
	{
		int lowestTarget = -1;
		Tile checkTile;
		
		List<Tile> leastTargetedTiles = new List<Tile>();

		List<Tile> borderTiles = new List<Tile>();

		for (int i = 0; i < pathTile.connections.Count; i++)
		{
			if (pathTile.connections[i] != null)
			borderTiles.Add(pathTile.connections[i].tile);
		}

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
		
		return closestTile;
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

	public bool Buildable()
	{
		//Debug.Log("TILE IS BUILDABLE " + pathTile !=null);
		return (pathTile != null);
	}

	public bool Buildable(Building b)
	{
		if (buildingRestrictions.Length == 0 )
		{
			//default, no restrictions on buildings
			return true;
		}
		else
		{
			for (int i = 0; i < buildingRestrictions.Length; i ++)
			{
				if (b.GetType() == buildingRestrictions[i].GetType())
				{
					return true;
				}
			}
		}

		return false;
	}
}
