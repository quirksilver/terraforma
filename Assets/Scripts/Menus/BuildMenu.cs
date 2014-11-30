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

    public void BuildDome()
    {
        BuildBuilding(typeof(Dome));
    }

    public void BuildRocket()
    {
        BuildBuilding(typeof(Rocket));
    }

    public void BuildBuilding(System.Type type)
    {
        GameObject newObj = GameObject.Instantiate(buidingPlacementObject) as GameObject;
        newObj.GetComponent<BuildingPlacement>().Setup(type);
    }

    public void Refresh()
    {
        foreach (BuildingButton button in buildingButtons)
        {
            button.Refresh();
        }
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
        for (int i = 0; i < 20; i++)
        {
            GameObject newButton = GameObject.Instantiate(buttonTemp) as GameObject;
            newButton.transform.parent = buttonTemp.transform.parent;
            RectTransform rect = newButton.transform as RectTransform;
            rect.anchoredPosition = new Vector2(
                (i%2==0) ? 0 : rect.sizeDelta.x, 
                (i/2) * -rect.sizeDelta.y);

//            Debug.Log(buildings[0].size.ToString());

            newButton.GetComponent<BuildingButton>().Setup(buildings[i%2],this);
            buildingButtons.Add(newButton.GetComponent<BuildingButton>());
        }


        Canvas.ForceUpdateCanvases();

        gridRoot.sizeDelta = new Vector2(0, 19 * 77);
        buttonTemp.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1.0f;

        Refresh();
    }
}
