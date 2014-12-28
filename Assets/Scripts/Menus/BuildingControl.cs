using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingControl : MonoBehaviour 
{
    public CanvasGroup canvasGroup;
    public Text nameLabel;
    private Building building;

    public Tile touchStart;

    float alpha = 0;
    float targetAlpha = 0;

    bool over = false;

    public Toggle activeToggle;

	public BuildHarvesterButton buildHarvesterButton;

    public ResourceInfo runningCost;

    public ResourceInfo producesInfo;

	// Use this for initialization
	void Start () {
	
	}

    public void BuildingToggle()
    {

        building.unitActive = activeToggle.isOn;
    }

	public void BuildHarvesterButton()
	{
		building.CreateNewHarvester();
	}

    public void OnOver()
    {
        over = true;
    }

    public void OnExit()
    {
        over = false;
    }

	public void RemoveBuilding()
	{
		Map.instance.GetLevel ().RemoveBuilding (building);
		targetAlpha = 0.0f;
	}

	// Update is called once per frame
	void Update () {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        canvasGroup.alpha = alpha;

		if (Map.instance.Pause) 
		{
			targetAlpha = 0.0f;
			return;
		}

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Map.instance.GetTileOver();
        }

        if (Map.instance.GetLevel() == null)
        {
            targetAlpha = 0.0f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Map.instance.GetTileOver() == touchStart && Map.instance.GetTileOver() != null)
            {
                if (Map.instance.GetTileOver().building != null)
                {
                    building = Map.instance.GetTileOver().building;
                    nameLabel.text = building.GetComponent<Building>().DisplayName;
                    targetAlpha = 1.0f;
                    activeToggle.isOn = building.unitActive;
					buildHarvesterButton.gameObject.SetActive(building.harvesterPrefab != null);

                    runningCost.ClearResources();
                    producesInfo.ClearResources();

                    // Add reource info
                    runningCost.AddResource(building.runningCostAir, ResourceType.Air);
                    runningCost.AddResource(building.runningCostFood, ResourceType.Food);
                    runningCost.AddResource(building.runningCostHeat, ResourceType.Heat);
                    runningCost.AddResource(building.runningCostMetal, ResourceType.Metal);
                    runningCost.AddResource(building.runningCostWater, ResourceType.Water);

                    runningCost.Tick();

                    producesInfo.AddResource(building.produceAir, ResourceType.Air);
                    producesInfo.AddResource(building.produceFood, ResourceType.Food);
                    producesInfo.AddResource(building.produceHeat, ResourceType.Heat);
                    producesInfo.AddResource(building.produceMetal, ResourceType.Metal);
                    producesInfo.AddResource(building.produceWater, ResourceType.Water);

					if (buildHarvesterButton.gameObject.activeSelf)
						buildHarvesterButton.Setup(building.harvesterPrefab.GetComponent<ResourceHarvester>(), this);
                }
                else
                {
                    if (!over)
                    {
                        targetAlpha = 0.0f;
                    }
                }
            }
        }
	}
}
