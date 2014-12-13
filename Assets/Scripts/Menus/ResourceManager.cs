using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ResourceType
{
    Water,
    Heat,
    Air,
    Food,
    Metal,
    Count
}

public class ResourceManager : MonoSingleton<ResourceManager>
{

    public Text[] resourceLabels;
    public Image[] changeRoot;
    float alpha = 0;
    float targetAlpha = 0;

	// Use this for initialization
	void Start ()
    {
        Refresh();
	}

	// Update is called once per frame
	void Update () 
    {
        targetAlpha = Map.instance.GetLevel() == null ? 0.0f : 1.0f;
        alpha = Mathf.MoveTowards(alpha, targetAlpha, 0.1f);
        GetComponent<CanvasGroup>().alpha = alpha;

        Refresh();
	}

    public void Refresh()
    {
        if (Map.instance.GetLevel() == null)
        {
            return;
        }
        for (int i = 0; i < (int)ResourceType.Count; i++)
        {
            resourceLabels[i].text = Map.instance.GetLevel().GetResource((ResourceType)i).ToString();
        }
    }

    public void Tick()
    {
        for (int i = 0; i < (int)ResourceType.Count; i++)
        {
            changeRoot[i].color = (Map.instance.GetLevel().resourceChange[i] == 0) ? Color.clear : (Map.instance.GetLevel().resourceChange[i] < 0) ? Color.red : Color.green;

            string label = "";
            if (Map.instance.GetLevel().resourceChange[i] != 0)
            {
                label = Map.instance.GetLevel().resourceChange[i].ToString();
                if (Map.instance.GetLevel().resourceChange[i] > 0)
                {
                    label = "+" + label;
                }
            }
            changeRoot[i].GetComponentInChildren<Text>().text = label;
        }
    }
}
