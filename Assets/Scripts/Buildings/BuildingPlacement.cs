using UnityEngine;
using System.Collections;

public class BuildingPlacement : MonoBehaviour 
{
    public string buildingPrefabPath;
    public GameObject BuildingHUD;
    private GameObject newBuilding;
    Vector3 lastPos;
    Building building;

	private bool bluePrint;

    // Use this for initialization
	void Start () {
	}

    public void Setup(System.Type type, bool bp)
	{
		bluePrint = bp;

        // Instaniate new building and disable it's scripts
        buildingPrefabPath = "Buildings/" + type.ToString();
        newBuilding = Instantiate(Resources.Load(buildingPrefabPath)) as GameObject;
        building = newBuilding.GetComponent(type) as Building;
        building.enabled = false;
		building.Setup(Map.instance.tileMap);

		building.SetTransparency(true);
		if (bluePrint) {
						building.SetColour (new Color (0, 0, 255, 0.3f));
				}
		//newBuilding.GetComponentInChildren<MeshRenderer>().material.shader = transparentDiffuse;//.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);

		Map.instance.AddObjectToLevel(newBuilding);
		Map.instance.AddObjectToLevel(gameObject);

    }

	// Update is called once per frame
	void Update ()
    {
        // Cancel if paused
        if (Map.instance.Pause)
        {
            Destroy(gameObject);
            Destroy(newBuilding);
            return;
        }

        Vector3 newPos = Vector3.zero;

		newPos.x = Map.instance.GetMouseOver().x;
		newPos.z = Map.instance.GetMouseOver().z;
		newPos.y = 0.01f;



        transform.localPosition = newBuilding.transform.localPosition = newPos;

        if (lastPos != newPos)
        {
			Debug.Log(newPos);
			Debug.Log(Map.instance.GetMouseOver());

			if(!bluePrint)
			{
				building.SetColour(Map.instance.ValidateBuilding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()) ? new Color(0.0f,1.0f,0.0f,0.8f) : new Color(1.0f,0.0f,0.0f,0.8f));
			}
			else
			{
				building.SetColour(new Color(0,0,255,Map.instance.ValidateBuilding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()) ? 0.8f:0.3f));
			}
			//newBuilding.GetComponentInChildren<MeshRenderer>().material.color = Map.instance.ValidateBuilding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()) ? new Color(0.0f,1.0f,0.0f,0.8f) : new Color(1.0f,0.0f,0.0f,0.8f) ;
        }

        // When clicked enable stop movement and enable scripts
        if (Input.GetMouseButtonDown(0))
        {


			//Debug.Log("over tile " + Map.instance.GetTileOver());

			if(bluePrint)
			{
				//remove object
				Destroy(gameObject);
				Destroy(newBuilding);
			}
			else if (Map.instance.IsMouseOutOfBounds())
			{
				CancelBuildingPlacement();
			}
            else if (Map.instance.PlaceBuiding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()))
            {
                Destroy(gameObject);
                Building building = newBuilding.GetComponent<Building>();
                BuildingHUDControl.instance.NewHud(building);
                building.enabled = true;

               // newBuilding.GetComponentInChildren<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

				//Testing
				//building.CreateNewHarvester();
            }
        }
        else if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.C))
        {
            // Cancel placement
			CancelBuildingPlacement();

        }

		lastPos = newPos;
	}

	public void CancelBuildingPlacement()
	{
		
		//refund
		Map.instance.GetLevel().AddResource(building.buildCostAir, ResourceType.Air);
		Map.instance.GetLevel().AddResource(building.buildCostFood, ResourceType.Food);
		Map.instance.GetLevel().AddResource(building.buildCostHeat, ResourceType.Heat);
		Map.instance.GetLevel().AddResource(building.buildCostMetal, ResourceType.Metal);
		Map.instance.GetLevel().AddResource(building.buildCostWater, ResourceType.Water);
		
		//remove object
		Destroy(gameObject);
		Destroy(newBuilding);
	}
}
