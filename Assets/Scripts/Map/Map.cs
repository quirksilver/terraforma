using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoSingleton<Map> 
{
    public float TileSize = 1.0f;
    public int width;
    public int height;

    public Vector3 mouseOverTile { private set; get; }

    private List<Building> buildings;
    private BuildMenu buildMenu;

	private TileMap tileMap;

	// Use this for initialization
	void Awake () {
        buildings = new List<Building>();
        buildMenu = FindObjectOfType(typeof(BuildMenu)) as BuildMenu;
		tileMap = GetComponent<TileMap>();
	}

    public int GetBuildingsCount(System.Type type)
    {
        int count = 0;
        foreach (Building b in buildings)
        {
            if (b.GetType() == type)
            {
                count++;
            }
        }
        return count;
    }

    public bool ValidateBuilding(Building building, Vector3 pos)
    {
        bool valid = true;

		Debug.Log(building.footprint);

		List<Vector3> footprintTiles = building.footprint.tilePositions;

		Debug.Log(footprintTiles);

		for (int i = 0; i < footprintTiles.Count; i++)
		{
			Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);

			if (checkTile == null)
			{
				valid = false;
			}
			else if (checkTile.building != null)
			{
				valid = false;
			}
		}

        return valid;
    }

    public bool PlaceBuiding(Building building, Vector3 pos)
    {
        bool valid = true;

        valid = ValidateBuilding(building, pos);

        if (valid)
        {

			List<Vector3> footprintTiles = building.footprint.tilePositions;
			
			Debug.Log(footprintTiles);
			
			for (int i = 0; i < footprintTiles.Count; i++)
			{
				Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);
				
				checkTile.AssignBuilding(building);
			}

			building.footprint.hide();

            buildings.Add(building);
            buildMenu.Refresh();
        }

        return valid;
    }

    // Update is called once per frame
    void Update()
    {
	}

    public void MouseOver(Vector3 pos)
    {
        mouseOverTile = pos;
    }

    public Vector3 GetMouseOver()
    {
        return mouseOverTile;
    }

    public Tile GetTileOver()
    {
		return tileMap.GetTile(mouseOverTile);
    }
}
