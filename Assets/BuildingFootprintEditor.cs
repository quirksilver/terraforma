using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildingFootprint))]
public class BuildingFootprintEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		BuildingFootprint footprintScript = (BuildingFootprint)target;
		if(GUILayout.Button("Calculate Sprite Pivot"))
		{
			footprintScript.CalculatePivot();
		}
	}
}