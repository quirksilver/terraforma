using UnityEngine;
using System.Collections;

public class BuildMenu : MonoBehaviour 
{
    public GameObject buidingPlacementObject;

    public void BuildDome()
    {
        GameObject newObj = GameObject.Instantiate(buidingPlacementObject) as GameObject;
        newObj.GetComponent<BuildingPlacement>().Setup(typeof(Dome));
    }
}
