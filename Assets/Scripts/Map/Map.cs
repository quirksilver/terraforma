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

    public MapLinks mapLinks;
    public Globe globe;

	private float CurrentDustAlpha=0;
	private float TargetDustAlpha=0.5f;
	private float CurrentCloudAlpha=0;
	private float TargetCloudAlpha=0.0f;

	public Material DustMat;
	public Material CloudMat;

	// Use this for initialization
	void Awake () {
        buildMenu = FindObjectOfType(typeof(BuildMenu)) as BuildMenu;

		buildingControl = FindObjectOfType(typeof(BuildingControl)) as BuildingControl;
	}

	void Start() {


		GoToWorldMap();
	}

	public void UpdateAtmos(float i)
	{
		Color dustColor;
	    dustColor =	DustMat.color;
		dustColor.a = i;
		DustMat.color = dustColor;

		CloudMat.color = new Color(1.0f,1.0f,1.0f,(1.0f - i));
	}

	public void LoadLevel(Level levelToLoad)
	{
		TargetCloudAlpha = 0.0f;
		TargetDustAlpha = 0.0f;

        nextLevelArrow.color = Color.clear;
		Debug.Log ("LoadLevel " + levelToLoad); 

		//buildMenu.enabled = true;
		//buildingControl.enabled = true;

		level = levelToLoad;

		tileMap = level.GetComponentInChildren<TileMap>();

		SetLevelCollidersEnabled(false);

        timeInLevel = 0;

        level.storyEventManager.Check();

        ResourceManager.instance.Tick();

		buildMenu.Refresh ();
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
			TargetCloudAlpha = ((float)levelIndex / (float)levels.Length) * 0.5f;
			TargetDustAlpha = 0.5f - TargetCloudAlpha;

            Vector3 diff = 
                levels[levelIndex - 1].GetComponent<Level>().centerPos - 
                levels[levelIndex].GetComponent<Level>().centerPos;

            Vector3 pos1 = levels[levelIndex - 1].GetComponent<Level>().centerPos - (diff.normalized * 7.0f);
            Vector3 pos2 = levels[levelIndex].GetComponent<Level>().centerPos + (diff.normalized * 7.0f);

            mapLinks.drawLine(
                pos1,
                pos2);

            GoToWorldMap();
        }
    }

	public void GoToWorldMap()
	{
		//testing pause functionality
		//Pause = true;

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
		bool canBuildOnResource = true;
		int requiredResources = 0;
		List<Vector3> footprintTiles = building.footprint.tilePositions;
	

		for (int i = 0; i < footprintTiles.Count; i++)
		{
			Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);

			if (checkTile == null)
			{
				valid = false;
			}
			else if (checkTile.building != null || !checkTile.Buildable(building))
			{
				valid = false;
			}

			if (checkTile is ResourceTile)
			{
				if (building.numberResourceTilesRequired > 0)
				{
					if ((checkTile as ResourceTile).resourceType == building.requiredResourceTileType)
						requiredResources ++;
				}
				else 
				{
					canBuildOnResource = false;
				}

			}
		}

		if (requiredResources < building.numberResourceTilesRequired) 
			canBuildOnResource = false;

		return valid && canBuildOnResource;
	}

    public bool PlaceBuiding(Building building, Vector3 pos)
    {
        bool valid = true;

        valid = ValidateBuilding(building, pos);

        if (!valid)
        {
            return false;
        }
        buildMenu.Refresh();
        level.PlaceBuiding(building, pos);
        return true;
    }

    // Update is called once per frame
    void Update()
    {
		float cloudSpeed = 0.1f; 
		CurrentDustAlpha = Mathf.MoveTowards (CurrentDustAlpha, TargetDustAlpha, cloudSpeed * Time.deltaTime);
		CurrentCloudAlpha = Mathf.MoveTowards (CurrentCloudAlpha, TargetCloudAlpha, cloudSpeed * Time.deltaTime);

		Color tempColor = DustMat.color;
		tempColor.a = CurrentDustAlpha;
		DustMat.color = tempColor;

		tempColor = CloudMat.color;
		tempColor.a = CurrentCloudAlpha;
		CloudMat.color = tempColor;

        if (level != null && Pause == false)
        {
            nextLevelArrow.color = Color.clear;
            timeInLevel += Time.deltaTime;
            tickTimer += Time.deltaTime;

			if (tickTimer > tickPeriod - 1)
			{
				if (!MusicPlayer.instance.playing && MusicPlayer.instance.ready) MusicPlayer.instance.Setup();
			}

            if (tickTimer > tickPeriod)
            {
                tickTimer -= tickPeriod;
                level.Tick();
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
			CompleteLevel();
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
