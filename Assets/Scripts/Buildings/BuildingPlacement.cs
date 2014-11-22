using UnityEngine;
using System.Collections;

public class BuildingPlacement : MonoBehaviour 
{
    Map map;
    public string buildingPrefabPath;
    private GameObject newBuilding;

    // Use this for initialization
	void Start () {
	}

    public void Setup(System.Type type)
    {
        buildingPrefabPath = "Buildings/" + type.ToString();
        newBuilding = Instantiate(Resources.Load(buildingPrefabPath)) as GameObject;
        Building building = newBuilding.GetComponent(type) as Building;
        building.enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        transform.localPosition = newBuilding.transform.localPosition = newPos;

        //Create building
        if (Input.GetMouseButtonDown(0))
        {
            Destroy(gameObject);
        }
	}
}
