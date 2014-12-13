using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildingHUD : MonoBehaviour {

    public Transform followPoint;
    public GameObject resTemp;
    public Image warning;
    float warningAlpha;
    private List<ResParticle> resIcons;

    public Sprite[] resTextures;

    private Building building;

    int chain = 0;

	// Use this for initialization
	public void Setup (Building b) 
    {
        resTemp.SetActive(false);
        resIcons = new List<ResParticle>();
        building = b;
	}
	
	// Update is called once per frame
	void Update () 
    {
        chain = 0;
        if (followPoint != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followPoint.transform.position);
        }

        if (resIcons != null)
        {
            for(int i=resIcons.Count-1;i>=0;i--)
            {
                resIcons[i].time += Time.deltaTime;
                resIcons[i].transform.localPosition = new Vector3(0, resIcons[i].time * 100.0f, 0);
                float alpha = Mathf.Sin(Mathf.PI * resIcons[i].time);
                if (resIcons[i].time < 0)
                {
                    alpha = 0.0f;
                }
                resIcons[i].GetComponent<CanvasGroup>().alpha = alpha * 2;

                if (resIcons[i].time > 1.0f)
                {
                    Destroy(resIcons[i].gameObject);
                    resIcons.RemoveAt(i);
                }
            }
        }

        if (!building.buildingActive || !building.resourcesAvailable)
        {
            warningAlpha = Mathf.Sin(Time.time);
            if (warningAlpha < 0)
            {
                warningAlpha = 0 - warningAlpha;
            }
        }
        else if (warningAlpha>0)
        {
            warningAlpha -= Time.deltaTime;
        }
        warning.color = new Color(1.0f, 0.0f, 0.0f, warningAlpha);
	}

    public void AddRes(int ammount, ResourceType type)
    {
        if (ammount == 0)
        {
            return;
        }

        GameObject newIcon = GameObject.Instantiate(resTemp) as GameObject;
        newIcon.transform.parent = transform;
        newIcon.transform.localScale = Vector3.one;
        newIcon.SetActive(true);
        ResParticle resPart = newIcon.GetComponent<ResParticle>();
        resPart.time = chain * -0.5f ;
        resPart.icon.sprite = resTextures[(int)type];
        resPart.label.text = ammount.ToString();
        resIcons.Add(newIcon.GetComponent<ResParticle>());

        chain++;
    }
}
