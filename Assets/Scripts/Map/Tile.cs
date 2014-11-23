using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public Vector2 coords;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseOver()
    {
        Map.instance.MouseOver(coords);
    }
}
