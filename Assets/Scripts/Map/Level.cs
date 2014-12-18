using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TileMap tileMap { get; set; }
	public List<Building> buildings { get; set; }
	public int id;

	private PerspectiveSwitcher switcher;
    private int[] resourceAmmount;
    public int[] resourceChange;

    public StoryEventManager storyEventManager { get; private set; }

    public Vector3 centerPos { private set; get; }

	// Use this for initialization
	void Awake () {
	
		buildings = new List<Building>();
		tileMap = GetComponentInChildren<TileMap>() as TileMap;
        tileMap.setTiles();

        foreach (Building build in GetComponentsInChildren<Building>())
        {
            build.Setup(tileMap);
            BuildingHUDControl.instance.NewHud(build);
            PlaceBuiding(build,build.transform.localPosition);
        }

		switcher = Camera.main.GetComponent<PerspectiveSwitcher>();

        resourceAmmount = new int[(int)ResourceType.Count];
        resourceChange = new int[(int)ResourceType.Count];

        resourceAmmount[(int)ResourceType.Metal] = 200;

        storyEventManager = GetComponent<StoryEventManager>();

        centerPos = collider.bounds.center;
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
                        requiredResources++;
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

    public void PlaceBuiding(Building building, Vector3 pos)
    {
        Debug.Log("PLACE AT " + pos);


            List<Vector3> footprintTiles = building.footprint.tilePositions;

            Debug.Log(footprintTiles);

            int i;

            for (i = 0; i < footprintTiles.Count; i++)
            {
                Debug.Log("TILECOUNT"+tileMap.tiles.Count);
                Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);

                checkTile.AssignBuilding(building);
            }

            tileMap.UpdateConnections();

            building.GetBorderTilePositions();

            building.footprint.hide();

            buildings.Add(building);

            for (i = 0; i < buildings.Count; i++)
            {
                buildings[i].UpdateBorderTilePositions();
            }
    }
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown()
	{
		Debug.Log("MOUSE DOWN ON LEVEL " + id );

		Map.instance.LoadLevel(this);

		switcher.switchToOrtho(this.transform.FindChild("FocusPoint"));

	}

	public void Unload()
	{
		//disable everything
		//we maybe don't need this after all?? I can't remember what I was intending to put in it

	}

    //Resource functions

    public void AddResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] += ammount;
        resourceChange[(int)type] += ammount;
    }

    public int GetResource(ResourceType type)
    {
        return resourceAmmount[(int)type];
    }

    public void RemoveResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] -= ammount;
        resourceChange[(int)type] -= ammount;
        if (resourceAmmount[(int)type] < 0)
        {
            resourceAmmount[(int)type] = 0;
        }
    }

    public void Tick()
    {
        ResourceManager.instance.Tick();
        for (int i = 0; i < (int)ResourceType.Count; i++)
        {
            resourceChange[i] = 0;
        }
    }
}
