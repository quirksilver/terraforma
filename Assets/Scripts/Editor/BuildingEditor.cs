using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Building), true)]
public class BuildingEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Building script = (Building)target;
		if(GUILayout.Button("Create Split Sprite"))
		{
			script.CreateSplitSprite();
		}
	}
}