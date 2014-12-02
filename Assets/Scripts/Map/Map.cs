using UnityEngine;
using System;
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
	private BuildingControl buildingControl;

	private TileMap tileMap;
    private float tickTimer = 0;
    public float tickPeriod = 0;

	private Level level;

	public GameObject[] levels;

	// Use this for initialization
	void Awake () {
        buildings = new List<Building>();
        buildMenu = FindObjectOfType(typeof(BuildMenu)) as BuildMenu;

		buildingControl = FindObjectOfType(typeof(BuildingControl)) as BuildingControl;
		//tileMap = GetComponent<TileMap>();
	}

	void Start() {


		GoToWorldMap();
	}

	public void LoadLevel(Level levelToLoad)
	{
		Debug.Log ("LoadLevel " + levelToLoad); 

		buildMenu.enabled = true;
		buildingControl.enabled = true;

		level = levelToLoad;

		tileMap = level.GetComponentInChildren<TileMap>();
		buildings = level.buildings;

		SetLevelCollidersEnabled(false);

	}

	public void GoToWorldMap()
	{
		Camera.main.GetComponent<PerspectiveSwitcher>().switchToPerspective();

		buildMenu.enabled = false;
		buildingControl.enabled = false;

		SetLevelCollidersEnabled(true);
	}

	public void SetLevelCollidersEnabled(bool value)
	{
		for (int i = 0; i < levels.Length; i++)
		{
			levels[i].collider.enabled = value;
		}
	}

	public void AddObjectToLevel(GameObject obj)
	{
		obj.transform.parent = level.transform;

		obj.transform.rotation = level.transform.rotation;
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
        tickTimer += Time.deltaTime;
        if (tickTimer > tickPeriod)
        {
            tickTimer -= tickPeriod;
            foreach (Building build in buildings)
            {
                build.Tick();
            }
            if (buildMenu && buildMenu.enabled) buildMenu.Tick();
        }

		if (Input.GetKeyDown(KeyCode.Space))
		{
			GoToWorldMap();
		}
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
