using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Unit : MonoBehaviour
{
	public string DisplayName;

	//Res
	public int buildCostWater;
	public int buildCostHeat;
	public int buildCostAir;
	public int buildCostFood;
	public int buildCostMetal;
	
	public int runningCostWater;
	public int runningCostHeat;
	public int runningCostAir;
	public int runningCostFood;
	public int runningCostMetal;
	
	public int produceWater;
	public int produceHeat;
	public int produceAir;
	public int produceFood;
	public int produceMetal;


	protected BuildingHUD hud;

	[HideInInspector]
	public string eventName;
	
	protected Shader mainShader;
	protected Shader transparentDiffuse;

	public float buildingTime;
	protected float buildingTimer;

	[HideInInspector]
	public bool built = false;

	[HideInInspector]
	public bool unitActive=true;
	[HideInInspector]
	public bool resourcesAvailable = true;

	protected TileMap tileMap;
	
	public Material unitMat;

		protected List<Material> unitChildMats;

	protected Color mainCol;

	protected virtual void Awake()
	{
		transparentDiffuse = Shader.Find("Transparent/Diffuse");

		eventName = "BUILT" + DisplayName.ToUpper();

		
				if (!unitMat)
				{
						unitMat = GetComponentInChildren<MeshRenderer>().material;
				}
				else
				{
						MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
						unitChildMats = new List<Material>();

						foreach(MeshRenderer ren in childRenderers)
						{
								if (ren.sharedMaterial == unitMat)
								{
										unitChildMats.Add(ren.material);
								}
						}
						
				}

						

		mainCol = unitMat.color;

		mainShader = unitMat.shader;

	}

	
	// Update is called once per frame
	public virtual void Update () {
		
		if (!built)
		{
			buildingTimer += Time.deltaTime;
			
			float i = buildingTimer/buildingTime;
			
			Color color = mainCol;
			color.a = i + (0.2f * Mathf.Sin(Time.time*Mathf.PI*2));
			SetColour(color);
			
			if (buildingTimer > buildingTime)
			{
				SetTransparency(false);
				//SetColour(Color.white);
				SetToMainColour();
				built=true;
				
				StoryEventManager.SendEvent(eventName);

				if (Map.instance.GetLevel() && Map.instance.GetLevel().id == 1 && eventName == "BUILTNUTRIFARM")
					MusicPlayer.instance.ReceiveEvent("SPOOKYWIND");

				Map.instance.RefreshBuildMenu();
			}
		}
		
	}

	public void SetTransparency(bool isTransparent)
	{
		if (isTransparent)
		{
						if (unitChildMats != null)
						{
								foreach (Material mat in unitChildMats)
								{
										mat.shader = transparentDiffuse;
								}
						}
						else
						{
								unitMat.shader = transparentDiffuse;
						}
			

		}
		else
		{
						if (unitChildMats!= null)
						{
								foreach (Material mat in unitChildMats)
								{
										mat.shader = mainShader;
								}
						}
						else
						{
								unitMat.shader = mainShader;
						}
			
		}
	}

	public void SetToMainColour()
	{
				if (unitChildMats!= null)
				{
						foreach (Material mat in unitChildMats)
						{
								mat.color = mainCol;
						}
				}
				else
				{
						unitMat.color = mainCol;
				}
		
	}
	
	public void SetColour(Color col)
	{


				if (unitChildMats!= null)
				{
						foreach (Material mat in unitChildMats)
						{
								mat.color = col;
						}
				}
				else
				{
						if (!unitMat) unitMat = GetComponentInChildren<MeshRenderer>().material;
						unitMat.color = col;
				}
		
	}
	
	public virtual void Setup(TileMap t)
	{
		tileMap = t;
	}

	
	public bool CanBuild()
	{
		if (Map.instance.GetLevel().GetResource(ResourceType.Water) < buildCostWater ||
		    Map.instance.GetLevel().GetResource(ResourceType.Metal) < buildCostMetal ||
		    Map.instance.GetLevel().GetResource(ResourceType.Heat) < buildCostHeat||
		    Map.instance.GetLevel().GetResource(ResourceType.Food) < buildCostFood ||
		    Map.instance.GetLevel().GetResource(ResourceType.Air) < buildCostAir)
		{
			return false;
		}
		
		return true;
	}
	
	public virtual void Tick()
	{
		if (unitActive && built)
		{
			resourcesAvailable = true;
			
			if (runningCostAir > Map.instance.GetLevel().GetResource(ResourceType.Air)
			    || runningCostFood > Map.instance.GetLevel().GetResource(ResourceType.Food)
			    || runningCostHeat > Map.instance.GetLevel().GetResource(ResourceType.Heat)
			    || runningCostMetal > Map.instance.GetLevel().GetResource(ResourceType.Metal)
			    || runningCostWater > Map.instance.GetLevel().GetResource(ResourceType.Water))
			{
				resourcesAvailable = false;
			}
			
			if (resourcesAvailable)
			{
				Map.instance.GetLevel().AddResource(produceWater, ResourceType.Water);
				Map.instance.GetLevel().AddResource(produceHeat, ResourceType.Heat);
				Map.instance.GetLevel().AddResource(produceAir, ResourceType.Air);
				Map.instance.GetLevel().AddResource(produceFood, ResourceType.Food);
				Map.instance.GetLevel().AddResource(produceMetal, ResourceType.Metal);
				
				Map.instance.GetLevel().RemoveResource(runningCostWater, ResourceType.Water);
				Map.instance.GetLevel().RemoveResource(runningCostHeat, ResourceType.Heat);
				Map.instance.GetLevel().RemoveResource(runningCostAir, ResourceType.Air);
				Map.instance.GetLevel().RemoveResource(runningCostFood, ResourceType.Food);
				Map.instance.GetLevel().RemoveResource(runningCostMetal, ResourceType.Metal);

				//Dictionary<ResourceType, int> dict = new Dictionary<ResourceType, int>();

				List<KeyValuePair<ResourceType, int>> myList = new List<KeyValuePair<ResourceType, int>>();

				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Water, produceWater - runningCostWater));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Heat, produceHeat - runningCostHeat));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Air, produceAir - runningCostAir));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Food, produceFood - runningCostFood));
				myList.Add(new KeyValuePair<ResourceType, int>(ResourceType.Metal, produceMetal - runningCostMetal));

				
				myList.Sort((firstPair,nextPair) =>
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
				);

				foreach (KeyValuePair<ResourceType, int> pair in myList)
				{
					hud.AddRes(pair.Value, pair.Key);
				}
			}
		}
	}

	public void SetHUD(BuildingHUD h)
	{
		hud = h;
	}
	
	public void RemoveHud()
	{
		BuildingHUDControl.instance.removeHud (hud.gameObject);
	}

#if UNITY_EDITOR

	public void SetPrefabMaterialColor(Color col)
	{
		GameObject clone = PrefabUtility.InstantiatePrefab(this.gameObject) as GameObject;
		
		Material mat = clone.GetComponentInChildren<MeshRenderer>().sharedMaterial;
		
		Debug.Log(mat);
		
		mat.color = col;
		
		//mat.SetColor("_MainColor", col);
		
		PrefabUtility.ReplacePrefab(clone, PrefabUtility.GetPrefabParent(clone), ReplacePrefabOptions.ConnectToPrefab);	
		
		DestroyImmediate(clone);
		
		EditorUtility.SetDirty(mat);
		
	}
#endif
}

