using UnityEngine;
using System.Collections;

public class Globe : MonoBehaviour 
{
    public RaycastHit RotAtPoint(Vector3 pos)
    {
        RaycastHit hit;
        Vector3 dir = transform.position - pos;
        dir = dir.normalized;
        Debug.DrawRay(pos, dir, Color.red, 120.0f);
        int layerMask = 1 << 8;
        Physics.Raycast(pos, dir, out hit, 1000,layerMask);
        Debug.DrawRay(pos, hit.normal*10, Color.green, 120.0f);
        return hit;
    }
}
