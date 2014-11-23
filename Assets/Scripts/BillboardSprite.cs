﻿using UnityEngine;
using System.Collections;

public class BillboardSprite : MonoBehaviour {


	public GameObject camera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (camera == null)
        {
            camera = Camera.main.gameObject;
        }

		Vector3 lookDirection = camera.transform.forward;

		transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
	
	}
}
