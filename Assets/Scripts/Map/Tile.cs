﻿using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public Vector3 coords;
    public Building building { private set; get; }
	public int harvestersTargeting = 0;

	// Use this for initialization
	public virtual void Start () {
        building = null;

		coords = transform.localPosition;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

    public void AssignBuilding(Building b)
    {
        building = b;

	
		PathTile pTile = GetComponent<PathTile>();
		DestroyImmediate(pTile);

		Map.instance.tileMap.UpdateConnections();
    }

    public virtual void OnMouseOver()
    {
        Map.instance.MouseOver(coords);
    }
}
