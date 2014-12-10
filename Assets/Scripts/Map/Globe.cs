using UnityEngine;
using System.Collections;

public class Globe : MonoBehaviour 
{
    public RaycastHit RotAtPoint(Vector3 pos)
    {
        RaycastHit hit;
        Vector3 dir = transform.position - pos;
        dir = dir.normalized;
        int layerMask = 1 << 8;
        Physics.Raycast(pos - (dir*500), dir, out hit, Mathf.Infinity,layerMask);
        return hit;
    }
}
