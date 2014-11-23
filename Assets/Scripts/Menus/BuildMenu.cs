using UnityEngine;
using System.Collections;

public class BuildMenu : MonoBehaviour 
{
    public GameObject buidingPlacementObject;

    public void BuildDome()
    {
        BuildBuilding(typeof(Dome));
    }

    public void BuildRocket()
    {
        BuildBuilding(typeof(Rocket));
    }

    public void BuildBuilding(System.Type type)
    {
        GameObject newObj = GameObject.Instantiate(buidingPlacementObject) as GameObject;
        newObj.GetComponent<BuildingPlacement>().Setup(type);
    }
}
