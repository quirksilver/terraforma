using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryEventManager : MonoBehaviour 
{
    public List<StoryEvent> events = new List<StoryEvent>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    static void SendEvent(string eve)
    {
    }

    public void Check()
    {
        foreach (StoryEvent eve in events)
        {
            eve.Check();
        }
    }
}
