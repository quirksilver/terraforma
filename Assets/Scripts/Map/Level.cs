using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TileMap tileMap { get; set; }
	public List<Building> buildings { get; set; }
	public int id;

	private PerspectiveSwitcher switcher;
    private int[] resourceAmmount;

    public StoryEventManager storyEventManager { get; private set; }

    public Vector3 centerPos { private set; get; }

	// Use this for initialization
	void Awake () {
	
		buildings = new List<Building>();
		tileMap = GetComponent<TileMap>() as TileMap;

		switcher = Camera.main.GetComponent<PerspectiveSwitcher>();

        resourceAmmount = new int[(int)ResourceType.Count];

        resourceAmmount[(int)ResourceType.Metal] = 200;

        storyEventManager = GetComponent<StoryEventManager>();

        centerPos = collider.bounds.center;
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
		//we maybe don't need this after all?? I can't remember what I was intending to put in it

	}

    //Resource functions

    public void AddResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] += ammount;
    }

    public int GetResource(ResourceType type)
    {
        return resourceAmmount[(int)type];
    }

    public void RemoveResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] -= ammount;
    }
	
}
