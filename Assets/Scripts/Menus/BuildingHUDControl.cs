using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingHUDControl : MonoSingleton<BuildingHUDControl> {
    public GameObject tempHUD;
	List<GameObject> hudList = new List<GameObject>();


	// Use this for initialization
	void Start ()
    {
        tempHUD.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public BuildingHUD NewHud(Unit building)
    {
        GameObject newHUD = GameObject.Instantiate(tempHUD) as GameObject;
        newHUD.SetActive(true);
        newHUD.transform.parent = transform;
        newHUD.transform.localScale = Vector3.one;
        BuildingHUD hudComp = newHUD.GetComponent<BuildingHUD>();
        hudComp.followPoint = building.transform;
        building.SetHUD(hudComp);
        hudComp.Setup(building);
		hudList.Add (newHUD);
        return hudComp;
    }

	public void removeHud(GameObject hud)
	{
		hudList.Remove (hud);
		Destroy (hud);
	}

	public void ClearHuds()
	{
		foreach (GameObject gameObject in hudList) 
		{
			Destroy(gameObject);
		}
		hudList.Clear ();
	}
}
