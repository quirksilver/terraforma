using UnityEngine;
using System.Collections;

public class BuildingHUDControl : MonoSingleton<BuildingHUDControl> {
    public GameObject tempHUD;

	// Use this for initialization
	void Start ()
    {
        tempHUD.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public BuildingHUD NewHud(Building building)
    {
        GameObject newHUD = GameObject.Instantiate(tempHUD) as GameObject;
        newHUD.SetActive(true);
        newHUD.transform.parent = transform;
        newHUD.transform.localScale = Vector3.one;
        BuildingHUD hudComp = newHUD.GetComponent<BuildingHUD>();
        hudComp.followPoint = building.transform;
        building.SetHUD(hudComp);
        hudComp.Setup(building);
        return hudComp;
    }
}
