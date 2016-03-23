using UnityEngine;
using System.Collections;

public class DragMouseOrbit : MonoBehaviour
{
	public Transform Target;
	public float Distance = 5.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public float yMinLimit = -20.0f;
	public float yMaxLimit = 80.0f;

	public bool endingOrbit = false;

	private float x;
	private float y;
	private MatrixBlender blender;
	
	void Awake()
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.x;
		y = angles.y;

		Quaternion rotation = Quaternion.Euler(y, x, 0);
		Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -Distance)) + Target.position;
		
		transform.rotation = rotation;
		transform.position = position;

		if(GetComponent<Rigidbody>() != null)
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}
	
	void LateUpdate()
	{
		if (endingOrbit) 
		{
			if(blender == null)
			{
				blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
			}

			if(blender.IsRunning)
			{
				return;
			}

			Distance=Mathf.MoveTowards(Distance,150.0f,Time.deltaTime*5.0f);
			x += Time.deltaTime*-3.0f;
			y= Mathf.MoveTowards(y,0,Time.deltaTime*5.0f);
			Quaternion rotation = Quaternion.Euler (y, x, 0);
			Vector3 position = rotation * (new Vector3 (0.0f, 0.0f, -Distance)) + Target.position;
		
			transform.rotation = rotation;
			transform.position = position;
			return;
		}

		if(Target != null && Input.GetMouseButton(0))
		{
			x += (float)(Input.GetAxis("Mouse X") * xSpeed * 0.02f);
			y -= (float)(Input.GetAxis("Mouse Y") * ySpeed * 0.02f);
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -Distance)) + Target.position;
			
			transform.rotation = rotation;
			transform.position = position;
		}
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
