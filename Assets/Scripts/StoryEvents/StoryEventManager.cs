﻿using UnityEngine;
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

    public static void SendEvent(string eve)
    {
        Map.instance.GetLevel().storyEventManager.ReciveEvent(eve);
    }

    private void ReciveEvent(string eventString)
    {
        foreach (StoryEvent eve in events)
        {
            if (eve.trigger == eTrigger.Event)
            {
                if (eve.eventString == eventString)
                {
                    eve.ActivateEvent();
                }
            }
        }
    }

    public void Check()
    {
        foreach (StoryEvent eve in events)
        {
            eve.Check();
        }
    }
}