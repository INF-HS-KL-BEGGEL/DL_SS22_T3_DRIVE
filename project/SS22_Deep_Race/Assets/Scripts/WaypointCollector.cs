using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointCollector : MonoBehaviour
{
    public int currentWaypointIndex = 0;
    private Action<int, float> onCollectedWaypoint;

    public void RegisterOnCollectedWaypoint(Action<int, float> callback)
    {
        onCollectedWaypoint += callback;
    }

    public void ResetWaypointCollector()
    {
        currentWaypointIndex = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Waypoint")
        {
            WaypointInfo wi = other.GetComponent<WaypointInfo>();
            if(wi != null && wi.index == currentWaypointIndex)
            {
                wi.DisableWaypoint();
                currentWaypointIndex++;
                if (onCollectedWaypoint != null)
                {
                    onCollectedWaypoint.Invoke(wi.index, wi.distanceToLastWaypoint);
                }
          
            }
        }
    }
}
