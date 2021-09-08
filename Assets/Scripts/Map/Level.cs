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

	public int startingWater;
	public int startingHeat;
	public int startingAir;
	public int startingFood;
	public int startingMetal;

	public Building[] victoryRequirements;
	
	private List<Building> startingBuildings;

	// Use this for initialization
	void Awake () {
		startingBuildings = new List<Building> ();
		startingBuildings.AddRange (GetComponentsInChildren<Building> ());

		buildings = new List<Building>();
		tileMap = GetComponentInChildren<TileMap>() as TileMap;
        tileMap.setTiles();

		switcher = Camera.main.GetComponent<PerspectiveSwitcher>();

        resourceAmmount = new int[(int)ResourceType.Count];
        resourceChange = new int[(int)ResourceType.Count];

		resourceAmmount[(int)ResourceType.Water] = startingWater;
		resourceAmmount[(int)ResourceType.Heat] = startingHeat;
		resourceAmmount[(int)ResourceType.Air] = startingAir;
		resourceAmmount[(int)ResourceType.Food] = startingFood;
		resourceAmmount[(int)ResourceType.Metal] = startingMetal;

        storyEventManager = GetComponent<StoryEventManager>();

        centerPos = GetComponent<Collider>().bounds.center;
	}

	public void LevelReset()
	{
		Debug.Log (gameObject.name + " RESET");

		resourceAmmount[(int)ResourceType.Water] = startingWater;
		resourceAmmount[(int)ResourceType.Heat] = startingHeat;
		resourceAmmount[(int)ResourceType.Air] = startingAir;
		resourceAmmount[(int)ResourceType.Food] = startingFood;
		resourceAmmount[(int)ResourceType.Metal] = startingMetal;

		for(int i=buildings.Count-1;i>=0;i--)
		{
			if(!startingBuildings.Contains(buildings[i]))
			{
				RemoveBuilding(buildings[i]);
			}
		}

		foreach (Tile tile in tileMap.tiles) 
		{
			tile.harvestersTargeting = 0;
		}

		tileMap.ResetResourceTiles();

		StoryEventManager.SendEvent ("RESET");

	}

    /*public bool ValidateBuilding(Building building, Vector3 pos)
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
            else if (checkTile.building != null || !checkTile.Buildable())
            {
                valid = false;
            }

			if (!checkTile.Buildable(building))
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
    }*/

	public void RemoveBuilding(Building building)
	{
		building.ClearHarvesters ();
		building.RemoveHud ();
		building.footprint.gameObject.SetActive (true);
		List<Vector3> footprintTiles = building.footprint.tilePositions;
		
		int i;

		for (i = 0; i < footprintTiles.Count; i++)
		{
			Tile checkTile = tileMap.GetTile(building.transform.localPosition + footprintTiles[i]);

			Debug.Log(checkTile.transform.localPosition);
			checkTile.ClearBuilding();
			Debug.Log(checkTile.building);
		}
		
		buildings.Remove (building);

		for (i = 0; i < buildings.Count; i++)
		{
			buildings[i].UpdateBorderTilePositions();
		}

		tileMap.UpdateConnections();

		bool uniqueBuildingRemoved = true;

		for (i = 0; i < buildings.Count; i++)
		{
			if (buildings[i].GetType() == building.GetType())
			{
				uniqueBuildingRemoved = false;
			}
		}

		if (uniqueBuildingRemoved) MusicPlayer.instance.ReceiveEvent("REMOVE" + building.eventName);

		Destroy (building.gameObject);
	}
	
	public void PlaceBuiding(Building building, Vector3 pos)
    {
		if (building.footprint.tilePositions == null) {
				building.footprint.CalculatePivot(false);
		}

            List<Vector3> footprintTiles = building.footprint.tilePositions;

            int i;

            for (i = 0; i < footprintTiles.Count; i++)
            {
                Tile checkTile = tileMap.GetTile(pos + footprintTiles[i]);

			Debug.Log("FP : " + footprintTiles[i]);
			Debug.Log(pos + footprintTiles[i]);

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

		switcher.switchToOrtho(this.transform.Find("FocusPoint"));

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

	public void OnMouseEnter()
	{
		Cursor.SetCursor (Map.instance.levelCursor, new Vector2(13,30), CursorMode.Auto);
	}
	
	public void OnMouseExit()
	{
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
	}

    public void Tick()
    {
				ResourceManager.instance.Tick ();
				for (int i = 0; i < (int)ResourceType.Count; i++) {
						resourceChange [i] = 0;
				}

				if (victoryRequirements.Length != 0) {
						bool victory = true;
						foreach (Building building in victoryRequirements) {
								bool present = false;
								foreach (Building b2 in buildings) {
										if (b2.DisplayName == building.DisplayName && b2.built) {
												present = true;
										}
								}

								if (!present) {
										victory = false;
								}
						}

						if (victory) {
								StoryEventManager.SendEvent ("LEVELVICTORY");
						}
				}
		}
}
