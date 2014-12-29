using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResourceInfo : MonoBehaviour
{
    public Sprite[] resImages;

    public GameObject tempCell;
    public Image icon;

	private List<GameObject> cells = new List<GameObject>();

    private Dictionary<ResourceType, Text> labelDict = new Dictionary<ResourceType, Text>();

    void Start()
    {
        tempCell.SetActive(false);
    }

    public void Tick()
    {
        foreach(KeyValuePair<ResourceType,Text> pair in labelDict)
        {
            int value = int.Parse(pair.Value.text);
            pair.Value.color = (value > Map.instance.GetLevel().GetResource(pair.Key)) ? Color.red : Color.white;
        }
    }

	public void ClearResources()
	{
		for (int i = 0; i < cells.Count; i++)
		{
			Destroy(cells[i]);
		}

		labelDict.Clear();

		Canvas.ForceUpdateCanvases();
		
		LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		
		Canvas.ForceUpdateCanvases();
	}

    public void AddResource(int ammount, ResourceType type)
    {
        if (ammount == 0)
        {
            return;
        }

        GameObject newCell = GameObject.Instantiate(tempCell) as GameObject;
        newCell.SetActive(true);
        newCell.transform.parent = tempCell.transform.parent;
        newCell.transform.localScale = Vector3.one;
        newCell.transform.GetChild(0).GetComponent<Image>().sprite = resImages[(int)type];

		Text text = newCell.GetComponentInChildren<Text> ();
		if (text != null) 
		{
			newCell.GetComponentInChildren<Text> ().text = ammount.ToString ();
		}

		cells.Add (newCell);

        labelDict.Add(type,newCell.GetComponentInChildren<Text>());

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);

        Canvas.ForceUpdateCanvases();
    }
}