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

	public Vector3 orthoPos, perspectivePos;
	public Vector3 orthoRotEuler, perspectiveRotEuler;

	private Quaternion orthoRot, perspectiveRot;
	
	void Start()
	{
		aspect = (float) Screen.width / (float) Screen.height;
		ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
		perspective = Matrix4x4.Perspective(fov, aspect, near, far);
		camera.projectionMatrix = ortho;
		orthoOn = true;
		blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));

		orthoRot = Quaternion.Euler(orthoRotEuler);
		perspectiveRot = Quaternion.Euler(perspectiveRotEuler);
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			orthoOn = !orthoOn;
			if (orthoOn)
				blender.BlendToMatrix(ortho, 1f, false, transform.position, orthoPos, transform.rotation, orthoRot);
			else
				blender.BlendToMatrix(perspective, 1f, true, transform.position, perspectivePos, transform.rotation, perspectiveRot);
		}
	}
}