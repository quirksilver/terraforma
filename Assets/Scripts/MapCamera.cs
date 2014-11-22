using UnityEngine;
using System.Collections;

public class MapCamera : MonoBehaviour 
{
    Vector3 lastMousePos;
    bool dragging;
    public Map map;

	// Use this for initialization
	void Start () {
	
	}

    public void SetMap(Map _map)
    {
        map = _map;
        transform.localPosition = new Vector3(
            (map.width * map.TileSize)/2.0f, 
            (map.height * map.TileSize)/2.0f, 
            -10);
    }
	
	// Update is called once per frame
	void Update () 
    {
        //mouse drag
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = lastMousePos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newCameraPos = transform.localPosition + delta;

            // pos clamp
            newCameraPos.x = Mathf.Clamp(newCameraPos.x, 0, map.width * map.TileSize);
            newCameraPos.y = Mathf.Clamp(newCameraPos.y, 0, map.height * map.TileSize);
            transform.localPosition = newCameraPos;
        }

        // Zoom in/out
        if (Input.mouseScrollDelta != Vector3.zero)
        {
            float newSize = GetComponent<Camera>().orthographicSize - Input.mouseScrollDelta.y;
            newSize = Mathf.Clamp(newSize, 2.0f, 12.0f);
            GetComponent<Camera>().orthographicSize = newSize;
        }

        lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
