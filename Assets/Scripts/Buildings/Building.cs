using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public Vector2 size;

	public BuildingFootprint footprint;

    public Building[] prerequisites;


	protected virtual void Awake () {
	
		footprint = GetComponentInChildren<BuildingFootprint>() as BuildingFootprint;
		
		Debug.Log(footprint);
	
	}

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	protected void HideFootprint()
	{
		footprint.hide();
	}
}
