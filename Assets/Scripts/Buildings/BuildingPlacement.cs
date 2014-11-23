using UnityEngine;
using System.Collections;

public class BuildingPlacement : MonoBehaviour 
{
    public string buildingPrefabPath;
    private GameObject newBuilding;
    Vector3 lastPos;

    // Use this for initialization
	void Start () {
	}

    public void Setup(System.Type type)
    {
        // Instaniate new building and disable it's scripts
        buildingPrefabPath = "Buildings/" + type.ToString();
        newBuilding = Instantiate(Resources.Load(buildingPrefabPath)) as GameObject;
        Building building = newBuilding.GetComponent(type) as Building;
        building.enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = Vector3.zero;
        newPos.x = Map.instance.GetMouseOver().x * Map.instance.TileSize;
        newPos.z = Map.instance.GetMouseOver().y * Map.instance.TileSize;
        newPos.y = 0.0f;

        transform.localPosition = newBuilding.transform.localPosition = newPos;

        if (lastPos != newPos)
        {
            newBuilding.GetComponent<SpriteRenderer>().color = Map.instance.ValidateBuilding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()) ? Color.white : Color.red;
        }

        // When clicked enable stop movement and enable scripts
        if (Input.GetMouseButtonDown(0))
        {
            if (Map.instance.PlaceBuiding(newBuilding.GetComponent<Building>(), Map.instance.GetMouseOver()))
            {
                Destroy(gameObject);
                Building building = newBuilding.GetComponent<Building>();
                building.enabled = true;
            }
        }
	}
}
