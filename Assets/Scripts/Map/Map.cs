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

    private BuildMenu buildMenu;
	private BuildingControl buildingControl;

	public TileMap tileMap { private set; get; }
    private float tickTimer = 0;
    public float tickPeriod = 0;

	private Level level;

    public int levelIndex = 0;

	public GameObject[] levels;

    public bool Pause = false;

    public float timeInLevel;

    public SpriteRenderer nextLevelArrow;

	// Use this for initialization
	void Awake () {
        buildMenu = FindObjectOfType(typeof(BuildMenu)) as BuildMenu;

		buildingControl = FindObjectOfType(typeof(BuildingControl)) as BuildingControl;
	}

	void Start() {


		GoToWorldMap();
	}

	public void LoadLevel(Level levelToLoad)
	{
        nextLevelArrow.color = Color.clear;
		Debug.Log ("LoadLevel " + levelToLoad); 

		//buildMenu.enabled = true;
		//buildingControl.enabled = true;

		level = levelToLoad;

		tileMap = level.GetComponentInChildren<TileMap>();

		SetLevelCollidersEnabled(false);

        timeInLevel = 0;

        level.storyEventManager.Check();
	}

    public Level GetLevel()
    {
        return level;
    }

    public void CompleteLevel()
    {
        level = null;
        levelIndex++;
        if (levelIndex >= levels.Length)
        {
            SceneSwitcher.ChangeScene(2);
        }
        else
        {
            GoToWorldMap();
        }
    }

	public void GoToWorldMap()
	{
		//testing pause functionality
		Pause = true;

		Camera.main.GetComponent<PerspectiveSwitcher>().switchToPerspective();

		//buildMenu.enabled = false;
		//buildingControl.enabled = false;

		SetLevelCollidersEnabled(true);
	}

	public void SetLevelCollidersEnabled(bool value)
	{
		for (int i = 0; i < levels.Length; i++)
		{
			levels[i].collider.enabled = value ? (levelIndex==i) : false;
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
        if (level != null)
        {
            foreach (Building b in level.buildings)
            {
                if (b.GetType() == type)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public bool ValidateBuilding(Building building, Vector3 pos)
    {
        bool valid = true;
		List<Vector3> footprintTiles = building.footprint.tilePositions;

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

			int i;

			for (i = 0; i < footprintTiles.Count; i++)
			{
				Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);
				
				checkTile.AssignBuilding(building);
			}

			building.footprint.hide();
			
			building.GetBorderTilePositions();

            level.buildings.Add(building);
            buildMenu.Refresh();

            for (i = 0; i < level.buildings.Count; i++)
			{
                level.buildings[i].UpdateBorderTilePositions();
			}
        }

        return valid;
    }

    // Update is called once per frame
    void Update()
    {
        if (level != null && Pause == false)
        {
            nextLevelArrow.color = Color.clear;
            timeInLevel += Time.deltaTime;
            tickTimer += Time.deltaTime;
            if (tickTimer > tickPeriod)
            {
                tickTimer -= tickPeriod;
                foreach (Building build in level.buildings)
                {
                    build.Tick();
                }
                if (buildMenu && buildMenu.enabled) buildMenu.Tick();

                level.storyEventManager.Check();
            }
        }

        //Update next level arrow
        if (level == null)
        {
            Vector3 lookDirection = Camera.main.transform.forward;
            nextLevelArrow.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            nextLevelArrow.transform.position = levels[levelIndex].collider.bounds.center + (levels[levelIndex].transform.rotation * new Vector3(0, 2, 0));
            Color color = Color.red;
            color.a = Mathf.Sin(Time.time*2.0f)+1.0f;
            nextLevelArrow.color = color;
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
