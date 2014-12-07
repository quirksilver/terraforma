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
}
