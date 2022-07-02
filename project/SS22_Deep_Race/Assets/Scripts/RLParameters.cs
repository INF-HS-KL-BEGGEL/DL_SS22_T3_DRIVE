using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class RLParameters : MonoBehaviour
{
    [Header("Reinforcement Learning Parameters")]
    public bool  all_wheels_on_track = false;
    public bool  approaching_next_waypoint = false;
    public float distance_from_center;
    public float speed;
    public float track_completion_progression;
    public float steering_angle;

    [Header("Retrospective RL Parameters")]
    public bool more_waypoints_reached    = false;
    public bool reached_new_high_speed    = false;

    [Header("Custom RL Parameters")]
    public bool waypoint_passed = false;
    public float distance_to_next_waypoint;
    public Vector3 track_center_position;
    public Vector3 next_waypoint_position;
    public float alignment_with_next_waypoint;
    public float alignment_speed_ratio;

    [Header("RoundTime RL Parameters")]
    public float round_time;
    public int round_counter;
    public bool round_over = false;
    
    private float startTime;
    private float traveledDistance = 0.0f;
    
    [Header("Debug")]
    public bool showDebugEstimatedTrackCenter;
    private float maxProgress = 0.0f;
    private float currentMaxProgress = -0.01f;
    private float maxSpeed = 0.0f;
    private float currentMaxSpeed = 0.0f;

    private List<TrackInfo> touchedTrack = new List<TrackInfo>();
    private TrackInfo lastClosestTrack;
    int layerMaskGround = 1 << 7;

    private GameObject debugCenterSphere;

    public void ResetRLParameters(bool resetRoundCounter = false)
    {
        all_wheels_on_track = false;
        distance_from_center = 0.0f;
        speed = 0.0f;
        steering_angle = 0.0f;

        distance_to_next_waypoint = 0.0f;
        track_center_position = Vector3.zero;
        next_waypoint_position = Vector3.zero;
        round_time = 0.0f;
        traveledDistance = 0.0f;

        this.ResetRetrospectiveParmeters();
        
        if(resetRoundCounter)
        {
            round_counter = 0;
        }

        round_over = false;

        this.PrepareNextRound();
    }

    public void FinishedRound()
    {
        // round_over = true;
        round_counter++;
        this.PrepareNextRound();
        this.ResetRetrospectiveParmeters();
    }


    public void OnVehicleUpdate(Vector3 vehiclePosition, Vector3 vehicleVelocity, AxleInfo[] vehicleAxleInfos, TrackInfo[] trackInfos)
    {
        all_wheels_on_track = AllWheelsOnTrack(vehicleAxleInfos);
        distance_from_center = GetDistanceFromCenter(vehiclePosition, trackInfos);
        approaching_next_waypoint = IsDrivingForward(vehiclePosition, vehicleVelocity);
        speed = GetSpeed(vehicleVelocity);
        steering_angle = GetSteerAngle(vehicleAxleInfos);
        
        reached_new_high_speed = speed > maxSpeed;
        if (speed > currentMaxSpeed){
            currentMaxSpeed = speed;
        }

        if (waypoint_passed){
            waypoint_passed = false;
        }

        distance_to_next_waypoint = Vector3.Distance(vehiclePosition, next_waypoint_position);

        if(!round_over)
        {
            round_time = Time.time - startTime;
        }
    }


    public void OnWaypointPassed(int passedWaypointIndex, float distanceToLastWaypoint, int waypointsCount, float waypointsTrackLength, Vector3 nextWaypointPosition)
    {   
        waypoint_passed = true;
        if (passedWaypointIndex == 0)
        {
            startTime = Time.time;
        }

        traveledDistance += distanceToLastWaypoint;
        track_completion_progression = traveledDistance / waypointsTrackLength - round_counter;
        this.next_waypoint_position = nextWaypointPosition;

        currentMaxProgress = track_completion_progression + round_counter;
        more_waypoints_reached = currentMaxProgress > maxProgress;
    }

    public TrackInfo GetLastClosestTrack()
    {
        return lastClosestTrack;
    }

    private float GetSpeed(Vector3 velocity)
    {
        float velocityValue = velocity.magnitude;
        if (this.approaching_next_waypoint){
            return velocityValue;
        }
        return -velocityValue;
    }

    private bool IsDrivingForward(Vector3 vehiclePosition, Vector3 vehicleVelocity)
    {
        Vector3 dirToTarget = (this.next_waypoint_position - vehiclePosition).normalized;
        alignment_with_next_waypoint = Vector3.Dot(dirToTarget, vehicleVelocity);
        alignment_speed_ratio = alignment_with_next_waypoint / this.speed;

        return alignment_with_next_waypoint > 1.00f;
    }

    private float GetSteerAngle(AxleInfo[] axleInfos)
    {
        float result = 0.0f;

        foreach (var axleInfo in axleInfos)
        {
            var wcLeft = axleInfo.leftWheel.GetComponent<WheelCollider>();
            var wcRight = axleInfo.rightWheel.GetComponent<WheelCollider>();

            if (axleInfo.steering)
            {
                result = Mathf.Max(wcLeft.steerAngle, wcRight.steerAngle);
            }
        }

        return result;
    }

    private bool AllWheelsOnTrack(AxleInfo[] axleInfos)
    {
        touchedTrack.Clear();
        bool OneWheelIsOffTrack = false;

        foreach (var axleInfo in axleInfos)
        {
            OneWheelIsOffTrack |= WheelIsNotOnTrack(axleInfo.leftWheel);
            OneWheelIsOffTrack |= WheelIsNotOnTrack(axleInfo.rightWheel);
        }

        return !OneWheelIsOffTrack;
    }

    private bool WheelIsNotOnTrack(GameObject wheel)
    {
        bool result = true;

        WheelCollider wheelCollider = wheel.GetComponent<WheelCollider>();
        if (wheelCollider != null)
        {
            Vector3 worldPosCollider;
            Quaternion worldRotCollider;
            wheelCollider.GetWorldPose(out worldPosCollider, out worldRotCollider);

            // Debug.DrawRay(pos, Vector3.down * 3, Color.red, 1.0f);

            RaycastHit raycastHit;
            if (Physics.Raycast(worldPosCollider, Vector3.down, out raycastHit, 3.0f, layerMaskGround))
            {
                if (raycastHit.collider.CompareTag("Track"))
                {
                    result = false;
                    TrackInfo ti = raycastHit.collider.gameObject.GetComponent<TrackInfo>();
                    if (ti != null)
                    {
                        touchedTrack.Add(ti);
                    }
                }
            }

            /*
            WheelHit wheelHit;
            if (wheelCollider.GetGroundHit(out wheelHit))
            {
                if (wheelHit.collider.CompareTag("Track"))
                {
                    result = false;

                    TrackInfo ti = wheelHit.collider.gameObject.GetComponent<TrackInfo>();
                    if (ti != null)
                    {
                        touchedTrack.Add(ti);
                    }
                }
            }*/
        }

        return result;
    }



    private void Start()
    {

        if (showDebugEstimatedTrackCenter)
        {
            debugCenterSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugCenterSphere.GetComponent<Collider>().enabled = false;
            debugCenterSphere.name = "debugCenterSphere";
        }
    }

    private float GetDistanceFromCenter(Vector3 vehiclePosition, TrackInfo[] trackInfos)
    {
        float result = 0.0f;

        ClosestTrackResult closestTrackResult = GetClosestTrackInfo(vehiclePosition, trackInfos);
        if(closestTrackResult != null)
        {
            result = closestTrackResult.closestTrackDistance;
            if(showDebugEstimatedTrackCenter)
            {
                debugCenterSphere.transform.position = closestTrackResult.closestTrackPosition;
            }
        }
        
        return result;
    }

    public Vector3 FindClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point, bool clamp = true)
    {
        Vector3 a = point - start;
        Vector3 b = end - start;

        float r = Vector3.Dot(a, b) / b.magnitude;

        if(clamp)
        {
            r = Mathf.Clamp(r, 0f, b.magnitude);
        }

        return start + b.normalized * r; ;
    }


    public Vector3 FindClosestPointOnCurve(Vector3 start, Vector3 center, Vector3 end, Vector3 point)
    {
        Vector3 firstPart = FindClosestPointOnLine(start, center, point);
        Vector3 secondPart = FindClosestPointOnLine(center, end, point);


        Vector3 result;

        if(Vector3.Distance(firstPart, point) < Vector3.Distance(secondPart, point))
        {
            result = firstPart;
        }
        else
        {
            result = secondPart;
        }

        return result;
    }


    private ClosestTrackResult GetClosestTrackInfo(Vector3 vehiclePosition, TrackInfo[] trackInfos)
    {
        float closestTrackDistance = float.MaxValue;
        float currentTrackDistance;

        TrackInfo closestTrack = null;

        Vector3 closestTrackPoint = vehiclePosition;

        TrackInfo[] usedTrackInfos = trackInfos;

        foreach (TrackInfo ti in usedTrackInfos)
        {
            Vector3 closestPointOnLines = ti.trackType == TrackInfo.TrackType.Curve ? FindClosestPointOnCurve(ti.trackStartPosition, ti.trackCenterPosition, ti.trackEndPosition, vehiclePosition) : FindClosestPointOnLine (ti.trackStartPosition, ti.trackEndPosition, vehiclePosition);
            currentTrackDistance = Vector3.Distance(vehiclePosition, closestPointOnLines);
            if (currentTrackDistance < closestTrackDistance)
            {
                closestTrack = ti;
                closestTrackDistance = currentTrackDistance;
                closestTrackPoint = closestPointOnLines;
            }
        }

     
        return closestTrack == null ? 
        null 
        : 
        new ClosestTrackResult()
        {
            closestTrack = closestTrack,
            closestTrackDistance = closestTrackDistance,
            closestTrackPosition = closestTrackPoint
        };
    }

    private void ResetRetrospectiveParmeters()
    {
        if (currentMaxProgress > maxProgress)
        {
            maxProgress = currentMaxProgress;
        }
        currentMaxProgress = 0;
        track_completion_progression = 0f;

        if (currentMaxSpeed > maxSpeed)
        {
            maxSpeed = currentMaxSpeed;
        }
        currentMaxSpeed = 0;
    }

    private void PrepareNextRound()
    {
        touchedTrack.Clear();
        lastClosestTrack = null;
        startTime = Time.time;
    }
}

