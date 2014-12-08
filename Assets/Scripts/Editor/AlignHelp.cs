using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Custom Commands/Align to sphere")]
    private static void AlignToSphere()
    {
        RaycastHit hit = Object.FindObjectOfType<Globe>().RotAtPoint(Selection.activeGameObject.transform.position);
        Selection.activeGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Selection.activeGameObject.transform.position = hit.point+hit.normal/10.0f;
    }
}