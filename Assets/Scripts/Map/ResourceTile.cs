using UnityEngine;
using System.Collections;

public class ResourceTile : Tile
{
	public ResourceType resourceType = ResourceType.Metal;
	public int resourceYieldPerTurn = 5;
	public int totalResourceYield = -1;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
	}

	// Update is called once per frame
	public override void Update ()
	{
		if (totalResourceYield == 0)
		{

			Map.instance.tileMap.RemoveResourceTile(this);

			GetComponentInChildren<Renderer>().material.color = Color.black;
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


}

