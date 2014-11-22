using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour 
{
    public float TileSize = 0.64f;
    Tile[,] tiles;
    public int width;
    public int height;

    public GameObject tempTile=null;

	// Use this for initialization
	void Start () {
        tiles = new Tile[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newTile = GameObject.Instantiate(tempTile) as GameObject;
                newTile.transform.parent = transform;
                newTile.transform.localPosition = new Vector3(x * TileSize, y * TileSize);
            }
        }

        Camera.mainCamera.GetComponent<MapCamera>().SetMap(this);
	}

    // Update is called once per frame
    void Update()
    {
	}
}
