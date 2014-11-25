using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour 
{
    private Building building;
    public Text label;
    private BuildMenu menu;

    public void Setup(Building b, BuildMenu m)
    {
        building = b;
        label.text = b.name;
        menu = m;
    }

    public void Refresh()
    {
        bool valid = true;
        foreach (Building b in building.prerequisites)
        {
            if (Map.instance.GetBuildingsCount(b.GetType())==0)
            {
                valid = false;
            }
        }
        GetComponent<Button>().interactable = valid;
    }

    public void Build()
    {
        menu.BuildBuilding(building.GetType());
    }
}
