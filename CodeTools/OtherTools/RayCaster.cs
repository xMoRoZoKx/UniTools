using System;
using System.Collections;
using System.Collections.Generic;
using UniTools;
using UnityEngine;

public class RayCaster
{
    public event Action<RaycastHit> OnRayEnter;
    public event Action<RaycastHit> OnRayStay;
    public event Action<RaycastHit> OnRayExit;

    RaycastHit previous;
    RaycastHit hit = new RaycastHit();

    public bool Cast(Vector3 startPos, Vector3 endPos, int layerMask = 0)
    {
        Physics.Raycast(startPos, startPos.Direction(endPos), out hit, Vector3.Distance(startPos, endPos), layerMask);
        ProcessCollision(hit);
        return hit.collider != null;
    }
    public bool ColliderExist(Vector3 startPos, Vector3 endPos, int layerMask = 0) => Physics.Raycast(startPos, startPos.Direction(endPos), out hit, Vector3.Distance(startPos, endPos), layerMask);
    public bool Cast(Vector3 origin, Vector3 direction, float distance, int layerMask = 0)
    {
        Physics.Raycast(origin, direction, out hit, distance, layerMask);
        ProcessCollision(hit);
        return hit.collider != null;
    }
    private void ProcessCollision(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            if (previous.collider != null)
            {
                DoEvent(OnRayExit, previous);
            }
        }
        else if (previous.collider == hit.collider)
        {
            DoEvent(OnRayStay, hit);
        }
        else if (previous.collider != null)
        {
            DoEvent(OnRayExit, previous);
            DoEvent(OnRayEnter, hit);
        }
        else
        {
            DoEvent(OnRayEnter, hit);
        }
        previous = hit;
    }
    private void DoEvent(Action<RaycastHit> action, RaycastHit hit) => action?.Invoke(hit);
}