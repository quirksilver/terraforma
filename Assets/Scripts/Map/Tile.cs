using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public Vector2 coords;
    public Building building { private set; get; }

	// Use this for initialization
	void Start () {
        building = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AssignBuilding(Building b)
    {
        building = b;
    }

    void OnMouseOver()
    {
        Map.instance.MouseOver(coords);
    }
}
