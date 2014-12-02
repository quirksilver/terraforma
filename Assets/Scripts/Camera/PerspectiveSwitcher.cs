using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
	private Matrix4x4   ortho,
	perspective;
	public float        fov     = 60f,
	near    = .3f,
	far     = 1000f,
	orthographicSize = 50f;
	private float       aspect;
	private MatrixBlender blender;
	private bool        orthoOn;

	private Vector3 orthoPos, perspectivePos;
	private Quaternion orthoRot, perspectiveRot;

	public Transform target;

	
	void Start()
	{
		//perspectivePos = target.position + new Vector3(0.0f, 0.0f, -90.0f);

		aspect = (float) Screen.width / (float) Screen.height;
		ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
		perspective = Matrix4x4.Perspective(fov, aspect, near, far);
		camera.projectionMatrix = perspective;
		orthoOn = false;
		blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));

		//transform.position = perspectivePos;
		//transform.rotation = perspectiveRot;
	}
	
	void Update()
	{

	}

	public void switchToOrtho(Transform focusPoint)
	{
		if (orthoOn) return;

		GetComponent<DragMouseOrbit>().enabled = false;

		orthoRot = focusPoint.parent.rotation * Quaternion.Euler(30, 45, 0);
		orthoPos = orthoRot * (new Vector3(0.0f, 0.0f, -110)) + focusPoint.position;

		orthoOn = true;
		if (orthoOn)
		{
			perspectivePos = transform.position;
			perspectiveRot = transform.rotation;
			
			blender.BlendToMatrix(ortho, 1f, false, perspectivePos, orthoPos, perspectiveRot, orthoRot);
		}

	}

	public void switchToPerspective()
	{
		if (!orthoOn) return;

		orthoOn = false;
		blender.BlendToMatrix(perspective, 1f, true, transform.position, perspectivePos, transform.rotation, perspectiveRot);

		GetComponent<DragMouseOrbit>().enabled = true;
	}
}