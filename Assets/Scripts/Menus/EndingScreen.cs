using UnityEngine;
using System.Collections;

public class EndingScreen : MonoBehaviour {

	CanvasGroup canvasGroup;
	float time = 0;

	// Use this for initialization
	void Start () {
		canvasGroup = GetComponent<CanvasGroup> ();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		if (time < 1) 
		{
			canvasGroup.alpha = Mathf.Cos ((time+1) * Mathf.PI);
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
		else 
		{
			canvasGroup.alpha = 1.0f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
	}

	public void EndButton()
	{
		SceneSwitcher.ChangeScene(2);
	}
}
