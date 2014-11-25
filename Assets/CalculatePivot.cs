using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CalculatePivot : MonoBehaviour {

	List<Vector3> grid;
	Vector3[] gridArray;
	Vector3[] transformedGrid;

	// Use this for initialization
	void Start () {

		int i;

		Debug.Log("Running calculate pivot");

		Quaternion rotation = Quaternion.Euler(0, 45, 0);
		Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

		grid = new List<Vector3>();

		foreach (Transform child in transform)
		{
			MeshFilter mf = child.GetComponent<MeshFilter>();

			for (i = 0; i < mf.mesh.vertices.Length; i++)
			{
				grid.Add(child.TransformPoint(mf.mesh.vertices[i]));
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

		Debug.Log(map(0.0f, bounds.min.z, bounds.max.z, 1.0f, 0.0f));

	}

	// Update is called once per frame
	void Update () {
	
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}
