using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public GameObject globe;
	public Material dustMat;
	public Material cloudMat;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		globe.transform.localEulerAngles = new Vector3 (90, Time.time, 0);

		float cloud = Mathf.Sin (Time.time*0.2f);
		cloud = (cloud + 1) * 0.25f;

		Color tempColor = cloudMat.color;
		tempColor.a = cloud;
		cloudMat.color = tempColor;

		tempColor = dustMat.color;
		tempColor.a = 0.5f - cloud;
		dustMat.color = tempColor;
	}

    public void StartGame()
    {
        SceneSwitcher.ChangeScene(1);
    }
}
