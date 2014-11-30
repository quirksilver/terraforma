using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResourceInfo : MonoBehaviour
{
    public Sprite[] resImages;

    public GameObject tempCell;
    public Image icon;

    private Dictionary<ResourceManager.ResourceType, Text> labelDict = new Dictionary<ResourceManager.ResourceType, Text>();

    void Start()
    {
        tempCell.SetActive(false);
    }

    public void Tick()
    {
        foreach(KeyValuePair<ResourceManager.ResourceType,Text> pair in labelDict)
        {
            int value = int.Parse(pair.Value.text);
            pair.Value.color = (value > ResourceManager.instance.GetResource(pair.Key)) ? Color.red : Color.white;
        }
    }

    public void AddResource(int ammount, ResourceManager.ResourceType type)
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
        newCell.GetComponentInChildren<Text>().text = ammount.ToString();
        labelDict.Add(type,newCell.GetComponentInChildren<Text>());

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);

        Canvas.ForceUpdateCanvases();
    }
}