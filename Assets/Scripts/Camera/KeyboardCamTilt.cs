using UnityEngine;
using System.Collections;

public class KeyboardCamTilt : MonoBehaviour {

	public float Distance = 5.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public float yMinLimit = 30.0f;
	public float yMaxLimit = 50.0f;
	public float xMinLimit = -90.0f;
	public float xMaxLimit = 90.0f;
	
	private float x = 45;
	private float y = 30;

	public bool tilting = false;
	private bool snapBack = false;

	private Vector3 targetPoint;
	private Transform targetTransform;

	private Vector3 startPos;
	private Quaternion startRot;

	private float snapTime = 0.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
		{
			if (!tilting) 
			{
				tilting = true;

				if (!snapBack) 
				{
					GetTargetPoint();
				}
				else
				{
					snapBack = false;
				}
			}

			Debug.Log("ROTATING FROM " + x + " " + y);

			x -= (float)(Input.GetAxis("Horizontal") * xSpeed * 0.02f);
			y += (float)(Input.GetAxis("Vertical") * ySpeed * 0.02f);



			y = ClampAngle(y, yMinLimit, yMaxLimit);
			x = ClampAngle(x, xMinLimit, xMaxLimit);

			Debug.Log("ROTATING TO " + x + " " + y);

			//orthoRot = focusPoint.parent.rotation * Quaternion.Euler(30, 45, 0);
			//orthoPos = orthoRot * (new Vector3(0.0f, 0.0f, -110)) + focusPoint.position;

			Quaternion rotation = targetTransform.parent.parent.rotation * Quaternion.Euler(y, x, 0);
			Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -Distance)) + targetPoint;
			
			transform.rotation = rotation;
			transform.position = position;

		}
		else if (tilting)
		{
			tilting = false;
			snapBack = true;

			snapTime = -1.0f;
		}

		else if (snapBack)
		{
			snapTime += Time.deltaTime;

			if (snapTime >= 0.0f) 
			{
				Quaternion slerpRot = Quaternion.Slerp(Quaternion.Euler(y, x, 0) , Quaternion.Euler(30, 45, 0), snapTime);

			//x = Mathf.Lerp(x, 45, snapTime);
			//y = Mathf.Lerp(x, 30, snapTime);

			Quaternion rotation = targetTransform.parent.parent.rotation * slerpRot; //Quaternion.Euler(y, x, 0);
			Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -Distance)) + targetPoint;
			
			transform.rotation = rotation;
			transform.position = position;

			if (snapTime >= 1) snapBack = false;

			x = slerpRot.eulerAngles.y;
			y = slerpRot.eulerAngles.x;
			}
		}



}

private void GetTargetPoint()
{
		/*Vector3 angles = transform.eulerAngles;
		x = angles.x;
		y = angles.y;*/

		startPos = transform.position;
		startRot = transform.rotation;

		x = 45;
		y = 30;

		Ray ray =  new Ray(transform.position, transform.forward);
		RaycastHit hit = new RaycastHit();

		if(Physics.Raycast (ray, out hit, 1000.0f, 1 << LayerMask.NameToLayer("Levels")))
		{
			targetPoint = hit.point;
			targetTransform = hit.transform;

			//Debug.Log("RAYCAST HIT!");
			Debug.Log(hit.point);
			Debug.Log(hit.transform);

			Debug.DrawRay(ray.origin, ray.direction, Color.green, 100.0f);
		}

		//Debug.Log("ANGLES " + (transform.rotation * Quaternion.Inverse(targetTransform.parent.parent.rotation)).eulerAngles); //(transform.eulerAngles - targetTransform.parent.parent.eulerAngles));

		Distance = Vector3.Distance(transform.position, targetPoint);

}

	public bool cameraIsTilting()
	{
		return (tilting || snapBack);
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if(angle < -360)
		{
			angle += 360;
		}
		if(angle > 360)
		{
			angle -= 360;
		}
		return Mathf.Clamp (angle, min, max);
	}
	
}
