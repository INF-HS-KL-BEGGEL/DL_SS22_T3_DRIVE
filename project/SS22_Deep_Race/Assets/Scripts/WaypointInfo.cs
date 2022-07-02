using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointInfo : MonoBehaviour
{
    public int index;

    public float distanceToNextWaypoint;
    public float distanceToLastWaypoint;

    public void DisableWaypoint()
    {
        this.gameObject.SetActive(false);
    }
}
