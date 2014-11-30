using UnityEngine;
using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class DragMouseOrbit : MonoBehaviour
{
	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	
	public float smoothTime = 2f;
	
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	
	float velocityX = 0.0f;
	float velocityY = 0.0f;

	Vector3 lastMousePos = Vector3.zero;
	
	// Use this for initialization
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
		
		// Make the rigid body not change rotation
		if (rigidbody)
		{
			rigidbody.freezeRotation = true;
		}
	}
	
	void LateUpdate()
	{
		if (target)
		{
			if (Input.GetMouseButton(0))
			{
				velocityX = Input.mousePosition.x - lastMousePos.x;//Input.GetAxis("Mouse X"); //* 0.02f;//distance * 0.02f;
				velocityY = Input.GetAxis("Mouse Y"); //* 0.02f;

				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				
				RaycastHit hit;
				
				Physics.Raycast(ray, out hit);
				
				Vector3 currentPos = hit.point;
				
				Physics.Raycast(camera.ScreenPointToRay(lastMousePos), out hit);
				
				Vector3 lastPos = hit.point;
				
				float dist = Vector3.Distance(currentPos, lastPos);
				
				Debug.Log(dist);
			}



			//float xAngle = 2* (Mathf.Asin((dist/2)/(85/2)));

			//Debug.Log(xAngle);
			
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			
			Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
			Quaternion rotation = toRotation;
			
			//distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);




			//Debug.Log(hit.collider);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			
			//velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			//velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

			lastMousePos = Input.mousePosition;
		}
		
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}