using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    Vector3 lastMousePos;
    Coroutine blendCoroutine = null;
    public Vector3 cameraOffset;
    private Vector3 cameraCenter;
	public Vector3 endingPos;

    public bool dragging = false;

	KeyboardCamTilt camTilt;

	void Start()
	{
		//perspectivePos = target.position + new Vector3(0.0f, 0.0f, -90.0f);

		aspect = (float) Screen.width / (float) Screen.height;
		ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
		perspective = Matrix4x4.Perspective(fov, aspect, near, far);
		GetComponent<Camera>().projectionMatrix = perspective;
		orthoOn = false;
		blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));

		camTilt= GetComponent<KeyboardCamTilt>();
		//transform.position = perspectivePos;
		//transform.rotation = perspectiveRot;
	}
	
	void Update()
	{
        if (orthoOn&&!Map.instance.Pause)
        {
            if (!blender.IsRunning)
            {

				if (!camTilt.enabled) camTilt.enabled = true;

                if (cameraCenter == Vector3.zero)
                {
                    cameraCenter = transform.localPosition;
                }

                //mouse drag
                if (Input.GetMouseButton(0)&&dragging)
                {
                    if (lastMousePos != Vector3.zero)
                    {
                        Vector3 delta = lastMousePos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        cameraOffset += delta;
                        //Vector3 newCameraPos = transform.localPosition + delta;
                        // pos clamp
                        Bounds mapBounds = Map.instance.tileMap.GetSize();

                        cameraOffset = Vector3.ClampMagnitude(cameraOffset, mapBounds.size.magnitude / 2.0f);

                        transform.localPosition = cameraCenter + cameraOffset;
                    }
                    lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {
                    dragging = false;
                    lastMousePos = Vector3.zero;
                }
            }
        }
	}

    public void StartDrag()
    {
		if (camTilt.cameraIsTilting()) return;

        dragging = true;
    }

	public void switchToOrtho(Transform focusPoint)
	{
		if (orthoOn) return;

        cameraCenter = Vector3.zero;
        cameraOffset = Vector3.zero;
		GetComponent<DragMouseOrbit>().enabled = false;

		orthoRot = focusPoint.parent.rotation * Quaternion.Euler(30, 45, 0);
		orthoPos = orthoRot * (new Vector3(0.0f, 0.0f, -110)) + focusPoint.position;

		orthoOn = true;
		if (orthoOn)
		{
			perspectivePos = transform.position;
			perspectiveRot = transform.rotation;

            blendCoroutine = blender.BlendToMatrix(ortho, 1f, false, perspectivePos, orthoPos, perspectiveRot, orthoRot);
		}

	}

	public void switchToEnding()
	{
		if (!orthoOn) return;
		
		orthoOn = false;
		blender.BlendToMatrix(perspective, 1f, true, transform.position, perspectivePos, transform.rotation, perspectiveRot);

		camTilt.Reset();
		camTilt.enabled = false;
		GetComponent<DragMouseOrbit>().enabled = true;
		GetComponent<DragMouseOrbit> ().endingOrbit = true;
	}

	public void switchToPerspective()
	{
		if (!orthoOn) return;

		orthoOn = false;
		blender.BlendToMatrix(perspective, 1f, true, transform.position, perspectivePos, transform.rotation, perspectiveRot);

		GetComponent<DragMouseOrbit>().enabled = true;
		camTilt.Reset();
		camTilt.enabled = false;
	}
}