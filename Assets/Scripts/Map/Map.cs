using UnityEngine;
using System.Collections;

public class Map : MonoSingleton<Map> 
{
    public float TileSize = 0.64f;
    Tile[,] tiles;
    public int width;
    public int height;

    public GameObject tempTile=null;

    public static Vector2 mouseOverTile { private set; get; }

	// Use this for initialization
	void Start () {
        tiles = new Tile[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newTile = GameObject.Instantiate(tempTile) as GameObject;
                newTile.transform.parent = transform;
                newTile.transform.localEulerAngles = Vector3.zero;
                newTile.transform.localPosition = new Vector3(x * TileSize, y * TileSize);
                newTile.gameObject.name = x.ToString() + "," + y.ToString();
                newTile.GetComponent<Tile>().coords = new Vector2(x, y);
                tiles[x,y] = newTile.GetComponent<Tile>();
            }
        }
	}

    public bool ValidateBuilding(Building building, Vector2 pos)
    {
        bool valid = true;

        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
            {
                int tx = (int)pos.x + x;
                int ty = (int)pos.y + y;

                if (tx < 0 ||
                    tx >= width ||
                    ty < 0 ||
                    ty >= height)
                {
                    valid = false;
                }
                else if (tiles[tx, ty].building != null)
                {
                    valid = false;
                }
            }
        }

        return valid;
    }

    public bool PlaceBuiding(Building building, Vector2 pos)
    {
        bool valid = true;

        valid = ValidateBuilding(building, pos);

        if (valid)
        {
            for (int x = 0; x < building.size.x; x++)
            {
                for (int y = 0; y < building.size.y; y++)
                {
                    tiles[(int)pos.x + x, (int)pos.y + y].AssignBuilding(building);
                }
            }
        }

        return valid;
    }

    // Update is called once per frame
    void Update()
    {
	}

    public void MouseOver(Vector2 pos)
    {
        mouseOverTile = pos;
    }

    public Vector2 GetMouseOver()
    {
        return mouseOverTile;
    }
}
