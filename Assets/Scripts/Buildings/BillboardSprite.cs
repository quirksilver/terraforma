using UnityEngine;
using System.Collections;

public class BillboardSprite : MonoBehaviour {


	private GameObject camera;

	// Use this for initialization
	void Start () {

		if (camera == null)
		{
			camera = Camera.main.gameObject;
		}
		
		Vector3 lookDirection = camera.transform.forward;
		
		transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
	
	}
	
	// Update is called once per frame
	void Update () {

        
	
	}
}
