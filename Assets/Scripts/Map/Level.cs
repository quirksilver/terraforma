using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TileMap tileMap { get; set; }
	public List<Building> buildings { get; set; }
	public int id;

	private PerspectiveSwitcher switcher;

	// Use this for initialization
	void Awake () {
	
		buildings = new List<Building>();
		tileMap = GetComponent<TileMap>() as TileMap;

		switcher = Camera.main.GetComponent<PerspectiveSwitcher>();
		

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

	}
}
