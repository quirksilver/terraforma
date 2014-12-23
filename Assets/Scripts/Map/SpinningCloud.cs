using UnityEngine;
using System.Collections;

public class SpinningCloud : MonoBehaviour 
{
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.localEulerAngles = new Vector3 ((Time.time * speed)%360.0f, 90, 90);
	}
}
