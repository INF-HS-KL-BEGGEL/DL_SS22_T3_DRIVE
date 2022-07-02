using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public RLParameters rLParameters;
    public bool showArrowHelper = true;
    public enum ArrowDirection { NextWaypoint }
    public ArrowDirection ArrowIndicatorTarget;
    public GameObject ArrowIndicatorGameObject;


    private Vector3 target;

    void Start()
    {
        if(!showArrowHelper)
        {
            ArrowIndicatorGameObject.SetActive(false);
        }

        if (rLParameters == null)
        {
            this.GetComponent<RLParameters>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (showArrowHelper && rLParameters != null && ArrowIndicatorGameObject != null)
        {
            if (ArrowIndicatorTarget == ArrowDirection.NextWaypoint)
            {
                target = rLParameters.next_waypoint_position;
                target.y = ArrowIndicatorGameObject.transform.position.y;
                ArrowIndicatorGameObject.transform.LookAt(target);
            }
        }

    }
}
