using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TileMap tileMap { get; set; }
	public List<Building> buildings { get; set; }
	public int id;

	public Vector3 cameraPos;
	public Vector3 cameraRot;

	// Use this for initialization
	void Start () {
	
		buildings = new List<Building>();
		tileMap = GetComponent<TileMap>() as TileMap;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		Map.instance.LoadLevel(this);
	}

	public void Unload()
	{
		//disable everything
	}
}
