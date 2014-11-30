using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceManager : MonoSingleton<ResourceManager>
{
	public enum ResourceType
		{
		Water,
		Heat,
		Air,
		Food,
		Metal,
		Count
		}

    public Text[] resourceLabels;
    private int[] resourceAmmount;

	// Use this for initialization
	void Start () {
        resourceAmmount = new int[(int)ResourceType.Count];

        resourceAmmount[(int)ResourceType.Metal] = 200;

        Refresh();
	}

    public void AddResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] += ammount;
        Refresh();
    }

    public int GetResource(ResourceType type)
    {
        return resourceAmmount[(int)type];
    }

    public void RemoveResource(int ammount, ResourceType type)
    {
        resourceAmmount[(int)type] -= ammount;
        Refresh();
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void Refresh()
    {
        for (int i = 0; i < (int)ResourceType.Count; i++)
        {
            resourceLabels[i].text = resourceAmmount[i].ToString();
        }
    }
}
