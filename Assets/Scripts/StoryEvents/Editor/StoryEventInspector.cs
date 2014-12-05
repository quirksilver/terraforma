using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StoryEventManager))]
public class LevelScriptEditor : Editor
{
    public Vector2 scrollPos = Vector2.zero;
    
    public override void OnInspectorGUI()
    {
        StoryEventManager myTarget = (StoryEventManager)target;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.border = new RectOffset(2,2,2,2);

        for (int i=myTarget.events.Count-1;i>=0;i--)
        {
            bool remove = false;
            StoryEvent eve = myTarget.events[i];

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Event");
            if (GUILayout.Button("X",GUILayout.Width(20)))
            {
                remove = true;
            }
            EditorGUILayout.EndHorizontal();

            //Trigger setting
            eve.trigger = (eTrigger) EditorGUILayout.EnumPopup("Trigger Type", eve.trigger);

            EditorGUILayout.Separator();
            if (eve.trigger == eTrigger.Time)
            {
                double seconds = eve.time.TotalSeconds;
                seconds = (double)EditorGUILayout.IntField("Time (Seconds)", (int)seconds);
                eve.time = System.TimeSpan.FromSeconds(seconds);
            }
            else
            {
                for (int i2 = 0; i2 < (int)ResourceType.Count; i2++)
                {
                    ResourceType type = (ResourceType) i2;
                    eve.resourceRequirements[i2] = EditorGUILayout.IntField(type.ToString(), eve.resourceRequirements[i2]);
                }
            }

            EditorGUILayout.Separator();

            //Set responce
            eve.EventResponce = (eEventResponse)EditorGUILayout.EnumPopup("Responce", eve.EventResponce);

            //if (eve.EventResponce == eEventResponse.Message)
            {
                for (int i2 = 0; i2 < eve.message.Count; i2++)
                {
                    EditorGUILayout.BeginVertical("box");
                    eve.message[i2].character = EditorGUILayout.TextField("Character", eve.message[i2].character);
                    eve.message[i2].message = EditorGUILayout.TextField("Message", eve.message[i2].message);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New Message"))
                {
                    eve.message.Add(new StoryMessage());
                }
                if (GUILayout.Button("Remove Message"))
                {
                    eve.message.RemoveAt(eve.message.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (remove)
            {
                myTarget.events.RemoveAt(i);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("NEW EVENT"))
        {
            myTarget.events.Add(new StoryEvent());
        }
    }
}
