using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildingHUD : MonoBehaviour {

    public Transform followPoint;
    public GameObject resTemp;
    public Image warning;
    float warningAlpha;
    private List<ResParticle> addResIcons;
    private List<ResParticle> subResIcons;

    public Sprite[] resTextures;

    private Unit building;

    int chain = 0;

	// Use this for initialization
	public void Setup (Unit b) 
    {
        resTemp.SetActive(false);
        addResIcons = new List<ResParticle>();
		subResIcons = new List<ResParticle>();
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

        if (addResIcons != null)
        {
            for(int i=addResIcons.Count-1;i>=0;i--)
            {
                /*addResIcons[i].time += Time.deltaTime;
                addResIcons[i].transform.localPosition = new Vector3(0, addResIcons[i].time * 100.0f, 0);
                float alpha = Mathf.Sin(Mathf.PI * addResIcons[i].time);
                if (addResIcons[i].time < 0)
                {
                    alpha = 0.0f;
                }
                addResIcons[i].GetComponent<CanvasGroup>().alpha = alpha * 2;

                if (addResIcons[i].time > 1.0f)
                {
                    Destroy(addResIcons[i].gameObject);
                    addResIcons.RemoveAt(i);
                }*/
				AnimateIcon(addResIcons[i], new Vector3(0, 100.0f, 0), addResIcons);
            }


        }

		if (subResIcons != null)
		{
			for(int i=subResIcons.Count-1;i>=0;i--)
			{
				/*subResIcons[i].time += Time.deltaTime;
				subResIcons[i].transform.localPosition = new Vector3(0, subResIcons[i].time * -100.0f, 0);
				float alpha = Mathf.Sin(Mathf.PI * subResIcons[i].time);
				if (subResIcons[i].time < 0)
				{
					alpha = 0.0f;
				}
				subResIcons[i].GetComponent<CanvasGroup>().alpha = alpha * 2;
				
				if (subResIcons[i].time > 1.0f)
				{
					Destroy(subResIcons[i].gameObject);
					addResIcons.RemoveAt(i);
				}*/

				AnimateIcon(subResIcons[i], new Vector3(0, -100.0f, 0), subResIcons);
			}
		} 

        if (!building.unitActive || !building.resourcesAvailable)
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

	private void AnimateIcon(ResParticle icon, Vector3 dir, List<ResParticle> containingList)
	{
		icon.time += Time.deltaTime;
		icon.transform.localPosition = icon.time * dir;
		float alpha = Mathf.Sin(Mathf.PI * icon.time);
		if (icon.time < 0)
		{
			alpha = 0.0f;
		}
		icon.GetComponent<CanvasGroup>().alpha = alpha * 2;
		
		if (icon.time > 1.0f)
		{
			Destroy(icon.gameObject);
			containingList.Remove(icon);
			//addResIcons.RemoveAt(i);
		}
	}

    public void AddRes(int amount, ResourceType type)
    {
        if (amount == 0)
        {
            return;
        }

        GameObject newIcon = GameObject.Instantiate(resTemp) as GameObject;
        newIcon.transform.parent = transform;
        newIcon.transform.localScale = Vector3.one *0.5f;
        newIcon.SetActive(true);
        ResParticle resPart = newIcon.GetComponent<ResParticle>();
        resPart.time = chain * -0.5f ;
		resPart.icon.sprite = resTextures[(int)type];
		resPart.label.text = amount.ToString();

		if (amount > 0)
		{
			addResIcons.Add(newIcon.GetComponent<ResParticle>());
		}
		else
		{
			resPart.icon.color = Color.red;
			resPart.label.color = Color.red;
			subResIcons.Add(newIcon.GetComponent<ResParticle>());
		}

        chain++;
    }

	/*public void SubtractRes(int amount, ResourceType type)
	{
		if (amount == 0)
		{
			return;
		}
		
		GameObject newIcon = GameObject.Instantiate(resTemp) as GameObject;
		newIcon.transform.parent = transform;
		newIcon.transform.localScale = Vector3.one *0.5f;
		newIcon.SetActive(true);
		ResParticle resPart = newIcon.GetComponent<ResParticle>();
		resPart.time = chain * -0.5f ;
		resPart.icon.sprite = resTextures[(int)type];
		resPart.icon.color = Color.red;
		resPart.label.color = Color.red;
		resPart.label.text = amount.ToString();
		subResIcons.Add(newIcon.GetComponent<ResParticle>());
		
		chain++;
	}*/
}
