using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceTile : Tile
{
	public ResourceType resourceType = ResourceType.Metal;
	public int resourceYieldPerTurn = 5;
	public int totalResourceYield = -1;

	public List<Tile> surroundingTiles = new List<Tile>();

	public bool empty = false;

	// Update is called once per frame
	public override void Update ()
	{
		if (totalResourceYield == 0)
		{
			Map.instance.tileMap.RemoveResourceTile(this);

			GetComponentInChildren<MeshRenderer>().material.mainTexture = Resources.LoadAssetAtPath<Texture2D>("Assets/Texture/ground_1.tga");

			//GetComponentInChildren<Renderer>().material.color = Color.black;
		}

	}

	public int HarvestResources(int bonus)
	{

		if (totalResourceYield == -1)
		{
			return resourceYieldPerTurn + bonus;
		}
		else
		{
			if (totalResourceYield >= (resourceYieldPerTurn + bonus))
			{
				totalResourceYield -= resourceYieldPerTurn + bonus;

				return resourceYieldPerTurn + bonus;
			}
			else
			{
				totalResourceYield = 0;

				return totalResourceYield;
			}


		}

	}

	public override void OnMouseEnter()
	{
		base.OnMouseEnter();
	}

	/*public void GetSurroundingTiles()
	{
		Vector3[] directions = new Vector3[8];
		directions[0] = Vector3.left;
		directions[1] = Vector3.forward;
		directions[2] = Vector3.right;
		directions[3] = Vector3.back;
		directions[4] = Vector3.left + Vector3.forward;
		directions[5] = Vector3.left + Vector3.back;
		directions[6] = Vector3.right + Vector3.forward;
		directions[7] = Vector3.right + Vector3.back;

		for (int i = 0; i < directions.Length; i ++)
		{

			Tile checkTile = Map.instance.tileMap.GetTile(coords + directions[i]);


			if (checkTile != null)
				surroundingTiles.Add( Map.instance.tileMap.GetTile(coords + directions[i]));
		}

		UpdateSurroundingTiles();
	}

	public void UpdateSurroundingTiles()
	{
		for (int i  = surroundingTiles.Count - 1; i >= 0; i--)
		{
			//Debug.Log("path tile " + Map.instance.tileMap.GetPathTile(surroundingTiles[i]));
			if (Map.instance.tileMap.GetPathTile(surroundingTiles[i]) == null)
			{
				surroundingTiles.RemoveAt(i);
			}
		}
	}

	public Tile GetLeastTargetedAdjacentTile()
	{
		surroundingTiles.Sort(TileTargetComparison);

		return surroundingTiles[0];
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
		else if (tileA.harvestersTargeting > tileB.harvestersTargeting)
		{
			return 1;
		}
	}
	*/
}

