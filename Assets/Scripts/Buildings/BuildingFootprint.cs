using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO; 
using System.Text;
using System.Xml;


public class BuildingFootprint : MonoBehaviour {

	private List<Vector3> grid;
	private Vector3[] gridArray;
	private Vector3[] transformedGrid;

	public Sprite sprite;

	private float pivotPoint;

	public List<Vector3> tilePositions { get; set; }
	
	// Use this for initialization
	void Awake () {

		if (tilePositions==null)
			CalculatePivot(false);

	}

	public void CalculatePivot(bool setPivot=true, bool saveXML = false){

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

		if (saveXML) SaveXml();
	}

	 

	public void SaveXml()
	{
		Debug.Log(tilePositions.Count);
		string xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n<footprint>\n";
		for (int i = 0; i < tilePositions.Count; i++)
		{
			xmlString += "\t<tile x='" + tilePositions[i].x + "' y='" + tilePositions[i].y + "' z='"+ tilePositions[i].z + "'/>\n";
		}

		xmlString += "</footprint>";

		Debug.Log(xmlString);
		Debug.Log(transform.parent.gameObject.name);

		//TextAsset asset = xmlString;

		File.WriteAllText("Assets/Resources/Buildings/" + transform.parent.gameObject.name + ".xml", xmlString);
		AssetDatabase.Refresh();

		//Load
		//TextAsset textXML = (TextAsset)Resources.Load("myxml.xml", typeof(TextAsset));

		/*AssetDatabase.CreateAsset(new TextAsset(), Application.dataPath + "/tests/test.xml");
		TextAsset textXML = (TextAsset)Resources.Load(Application.dataPath + "/tests/test.xml", typeof(TextAsset));


		XmlDocument xml = new XmlDocument();
		xml.LoadXml(textXML.text);

		//Simple Save
		xml.Save(AssetDatabase.GetAssetPath(textXML));*/

		/*
	     StreamWriter writer; 
	     FileInfo t = new FileInfo(Application.dataPath + "/tests/testXML.xml"); 
	     if(!t.Exists) 
	     { 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(xmlString); 
		writer.Close(); 
		Debug.Log("File written."); */
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
