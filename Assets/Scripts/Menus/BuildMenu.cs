using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildMenu : MonoBehaviour 
{
    public GameObject buidingPlacementObject;

    public GameObject buttonTemp;
    public RectTransform gridRoot;

    public Building[] buildings;

    private List<BuildingButton> buildingButtons;
    public UnityEngine.UI.ScrollRect scrollRect;

    float alpha = 0;
    float targetAlpha = 0;

    /*public void BuildDome()
    {
        BuildBuilding(typeof(Dome));
    }

    public void BuildRocket()
    {
        BuildBuilding(typeof(Rocket));
    }*/

	public void BuildBuilding(System.Type type, bool bluePrint=false)
    {
        GameObject newObj = GameObject.Instantiate(buidingPlacementObject) as GameObject;
        newObj.GetComponent<BuildingPlacement>().Setup(type,bluePrint);
    }

    public void Refresh()
    {
        foreach (BuildingButton button in buildingButtons)
        {
            button.Refresh();
        }
    }

	public void ResetButton()
	{
		Map.instance.ResetLevel ();
	}

    public void Tick()
    {
        foreach (BuildingButton button in buildingButtons)
        {
            button.cost.Tick();
        }
    }

    void Start()
    {
        //Building load
        buildingButtons = new List<BuildingButton>();
        for (int i = 0; i < buildings.Length; i++)
        {
            GameObject newButton = GameObject.Instantiate(buttonTemp) as GameObject;
            newButton.transform.parent = buttonTemp.transform.parent;
            /*RectTransform rect = newButton.transform as RectTransform;
            rect.anchoredPosition = new Vector2(
                0, 
                (i) * -rect.sizeDelta.y);*/

//            Debug.Log(buildings[0].size.ToString());

            newButton.GetComponent<BuildingButton>().Setup(buildings[i%buildings.Length],this);
            buildingButtons.Add(newButton.GetComponent<BuildingButton>());
        }
		

        gridRoot.sizeDelta = new Vector2(0, (buildings.Length) * 105);
        buttonTemp.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1.0f;

		Canvas.ForceUpdateCanvases();
        Refresh();
    }

    void Update()
    {
        targetAlpha = Map.instance.GetLevel() == null ? 0.0f : 1.0f;
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        GetComponent<CanvasGroup>().alpha = alpha;
    }
}
