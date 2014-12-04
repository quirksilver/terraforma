using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public Vector3 coords;
    public Building building { private set; get; }

	// Use this for initialization
	public virtual void Start () {
        building = null;

		coords = transform.localPosition;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

    public void AssignBuilding(Building b)
    {
        building = b;
    }

    public virtual void OnMouseOver()
    {
        Map.instance.MouseOver(coords);
    }
}
