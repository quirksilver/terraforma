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

    private bool fired = false;

    public void Check()
    {
        if (fired)
        {
            return;
        }

        bool passed = true;

        if (trigger == eTrigger.Resources)
        {
            for (int i = 0; i < (int)ResourceType.Count; i++)
            {
                if (Map.instance.GetLevel().GetResource((ResourceType)i) < resourceRequirements[i])
                {
                    passed = false;
                }
            }
        }
        else if (trigger == eTrigger.Time)
        {
            passed = Map.instance.timeInLevel > time;
        }

        if (passed)
        {
            fired = true;
            MessageWindow.instance.StartMessages(message, EventResponce == eEventResponse.WinLevel);
        }
    }
}