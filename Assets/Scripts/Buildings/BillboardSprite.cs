using UnityEngine;
using System.Collections;

public class BillboardSprite : MonoBehaviour {


	private GameObject billboardCam;

	// Use this for initialization
	void Start () {

		if (billboardCam == null)
		{
			billboardCam = Camera.main.gameObject;
		}
		


		Debug.Log("camera up " + billboardCam.transform.up);

		Vector3 lookDirection = billboardCam.transform.forward;
		transform.rotation = Quaternion.LookRotation(lookDirection, billboardCam.transform.up);
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
