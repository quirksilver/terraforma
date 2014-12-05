using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eTrigger
{
    Time,
    Resources,
}

public enum eEventResponse
{
    WinLevel,
    Message,
}

[System.Serializable]
public class StoryEvent 
{
    public eEventResponse EventResponce = eEventResponse.Message;
    public List<StoryMessage> message = new List<StoryMessage>();
    public eTrigger trigger = eTrigger.Time;
    public int[] resourceRequirements = new int[(int)ResourceType.Count];
    public System.TimeSpan time = System.TimeSpan.Zero;
}
