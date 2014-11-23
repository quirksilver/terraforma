using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public Vector2 size;
	public GameObject footprintPrefab;

	protected GameObject footprint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	protected void AttachFootprint() {

		if (!footprintPrefab)
		{
			Debug.LogError("No footprint prefab found for Building " + this.name);
		}

		footprint = (GameObject)Instantiate(footprintPrefab, transform.position + new Vector3(Map.instance.TileSize/2, 0, Map.instance.TileSize/2), Quaternion.identity);
		
		footprint.transform.parent = this.transform;


	}
}
