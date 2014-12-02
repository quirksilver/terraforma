using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingHUD : MonoBehaviour {

    public Transform followPoint;
    public GameObject resTemp;
    private List<ResParticle> resIcons;

    public Sprite[] resTextures;

    int chain = 0;

	// Use this for initialization
	void Start () 
    {
        resTemp.SetActive(false);
        resIcons = new List<ResParticle>();
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
	}

    public void AddRes(int ammount, ResourceManager.ResourceType type)
    {
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
