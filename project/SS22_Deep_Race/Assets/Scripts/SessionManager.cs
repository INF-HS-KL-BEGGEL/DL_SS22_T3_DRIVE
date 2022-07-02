using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public VehicleController vehicleController;
    public WaypointCollector waypointCollector;

    public Track track;
    public TrackWaypoints trackWaypoints;

    public RLParameters rLParameters;
    public RLDriver rlDriver;

    public UIStatsTracker stats;

    private void Start()
    {
        waypointCollector.RegisterOnCollectedWaypoint(OnPassedWaypoint);
    }

    private void FixedUpdate()
    {
        if(stats != null)
        {
            stats.SetTextTime(rLParameters.round_time, rLParameters.round_counter);
        }
    }

    public void VehicleUpdate(Vector3 vehiclePosition, Vector3 vehicleVelocity, AxleInfo[] vehicleAxleInfos)
    {
        rLParameters.OnVehicleUpdate(vehiclePosition, vehicleVelocity, vehicleAxleInfos, track.trackInfoList);
    }

    public void OnPassedWaypoint(int passedWaypointIndex, float distanceToLastWaypoint)
    {
        Vector3 nextWaypointPosition = Vector3.zero;

        if (passedWaypointIndex == trackWaypoints.WaypointsCount - 1)
        {
            OnAllWaypointsPassed();
        }
        else
        {
            nextWaypointPosition = trackWaypoints.waypointsPositions[passedWaypointIndex + 1];
        }

        rLParameters.OnWaypointPassed(passedWaypointIndex, distanceToLastWaypoint, trackWaypoints.WaypointsCount, trackWaypoints.WaypointsTrackLength, nextWaypointPosition);
    }

    public void OnAllWaypointsPassed()
    {
        rLParameters.FinishedRound();
        rlDriver.OnFinishedTrack();
    }

    public void VehicleAction(VehicleAction vehicleAction)
    {
        vehicleController.VehicleAction(vehicleAction);
    }

    public void ResetSession()
    {
        this.ResetIncludingCar(true);
    }

    public void NextRound()
    {
        waypointCollector.ResetWaypointCollector();
        trackWaypoints.ResetTrackWaypoints();
    }

    private void ResetIncludingCar(bool includingCar = false)
    {
        if (includingCar){
            vehicleController.ResetVehicleController();
        }

        waypointCollector.ResetWaypointCollector();
        trackWaypoints.ResetTrackWaypoints();
        rLParameters.ResetRLParameters(includingCar);
    }
}
