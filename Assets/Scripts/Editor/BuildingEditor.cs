using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Unit), true)]
public class BuildingEditor : Editor
{

	public string newColor = "";

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Unit script = (Unit)target;
		if(GUILayout.Button("Create Split Sprite") && script is Building)
		{
			(script as Building).CreateSplitSprite();
		}



		//string newColor = "000000";

		newColor = EditorGUILayout.TextField("Material Hex Color", newColor);

		//Debug.Log(newColor);
		//this.Repaint();

		if (GUILayout.Button("Set Hex Color"))
		{

			Debug.Log(newColor);
			script.SetPrefabMaterialColor(HexToColor(newColor));
		}

		
	}

	Color HexToColor(string hex)
	{
	byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
	byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
	byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
	return new Color32(r,g,b, 255);
	}


}