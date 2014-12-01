﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class BuildingFootprint : MonoBehaviour {

	private List<Vector3> grid;
	private Vector3[] gridArray;
	private Vector3[] transformedGrid;

	public Sprite sprite;

	private float pivotPoint;

	public List<Vector3> tilePositions { get; set; }
	
	// Use this for initialization
	void Start () {

		if (tilePositions==null)
			CalculatePivot(false);

	}

	public void CalculatePivot(bool setPivot=true){

		int i;

		transform.parent.position = Vector3.zero;

		tilePositions = new List<Vector3>();
		
		Debug.Log("Running calculate pivot");
		
		Quaternion rotation = Quaternion.Euler(0, 45, 0);
		Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
		
		grid = new List<Vector3>();
		
		foreach (Transform child in transform)
		{
			Debug.Log(child);

			tilePositions.Add(child.position);

			MeshFilter mf = child.GetComponentsInChildren<MeshFilter>(true)[0];

			Debug.Log(mf);
			
			for (i = 0; i < mf.sharedMesh.vertices.Length; i++)
			{
				grid.Add(mf.transform.TransformPoint(mf.sharedMesh.vertices[i]));
			}
			
		}
		
		gridArray = grid.ToArray();
		transformedGrid = new Vector3[gridArray.Length];
		
		
		
		for (i = 0; i < gridArray.Length; i++)
		{
			transformedGrid[i] = m.MultiplyPoint(gridArray[i]);
			
			Debug.Log(transformedGrid[i]);
		}
		
		Bounds bounds = new Bounds();
		
		for (i = 0; i < transformedGrid.Length; i++)
		{
			bounds.Encapsulate(transformedGrid[i]);
		}
		
		Debug.Log(bounds.min.z + ", " + bounds.max.z);

		pivotPoint = map(0.0f, bounds.min.z, bounds.max.z, 1.0f, 0.0f);

		Debug.Log(pivotPoint);

		if (setPivot) SetPivot();
	}

	void SetPivot() {

		string path = AssetDatabase.GetAssetPath(sprite.texture);
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.isReadable = true;

		TextureImporterSettings texSettings = new TextureImporterSettings();
		
		textureImporter.ReadTextureSettings(texSettings);
		texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
		texSettings.spritePivot = new Vector2(pivotPoint, 0);
		textureImporter.SetTextureSettings(texSettings);
		
		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
	}

	public void hide()
	{
		enabled = false;

		Renderer[] r = GetComponentsInChildren<Renderer>();

		for (int i = 0; i < r.Length; i++)
		{
			r[i].enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}