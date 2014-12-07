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

	// Use this for initialization
	void Start () {
	
	}

    public void BuildingToggle()
    {

        building.buildingActive = activeToggle.isOn;
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

	// Update is called once per frame
	void Update () {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        canvasGroup.alpha = alpha;

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
            if (Map.instance.GetTileOver() == touchStart)
            {
                if (Map.instance.GetTileOver().building != null)
                {
                    building = Map.instance.GetTileOver().building;
                    nameLabel.text = building.GetComponent<Building>().DisplayName;
                    targetAlpha = 1.0f;
                    activeToggle.isOn = building.buildingActive;
					buildHarvesterButton.gameObject.SetActive(building.harvesterPrefab != null);

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
