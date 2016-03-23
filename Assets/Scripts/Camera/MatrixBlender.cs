using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{
    public bool IsRunning;

	public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time, float duration, bool easeIn)
	{
		Matrix4x4 ret = new Matrix4x4();
		for (int i = 0; i < 16; i++){
			//ret[i] = Mathf.Lerp(from[i], to[i], time);

			if (easeIn)
			{
				ret[i] = easeInCubic(time, from[i], to[i]-from[i], duration);
			}
			else
			{
				ret[i] = easeOutCubic(time, from[i], to[i]-from[i], duration);
			}
		}
		return ret;
	}
	
	private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration, bool easeIn, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
	{
        IsRunning = true;
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			GetComponent<Camera>().projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration, duration, easeIn);
			GetComponent<Camera>().transform.position = Vector3.Slerp(startPos, endPos, (Time.time - startTime) / duration);
			GetComponent<Camera>().transform.rotation = Quaternion.Slerp(startRot, endRot, (Time.time - startTime) / duration);

			yield return 1;
		}
		GetComponent<Camera>().projectionMatrix = dest;
		GetComponent<Camera>().transform.position = endPos;
		GetComponent<Camera>().transform.rotation = endRot;
        IsRunning = false;
	}

	//cubic easing in - accelerating from zero velocity
		
		//time, start value, change in value, duration
	public static float easeInCubic(float t, float b, float c, float d) {
		t /= d;
		return c*t*t*t + b;
	}
	
	
	
	// cubic easing out - decelerating to zero velocity
	
	
	public static float easeOutCubic(float t, float b, float c, float d) {
		t /= d;
		t--;
		return c*(t*t*t + 1) + b;
	}
	
	public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration, bool easeIn, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
	{
		StopAllCoroutines();
		return StartCoroutine(LerpFromTo(GetComponent<Camera>().projectionMatrix, targetMatrix, duration, easeIn, startPos, endPos, startRot, endRot));
	}
}
