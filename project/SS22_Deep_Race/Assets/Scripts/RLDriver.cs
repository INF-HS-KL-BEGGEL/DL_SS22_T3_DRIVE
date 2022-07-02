using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System;

[RequireComponent(typeof(RLParameters))]
public class RLDriver : Agent
{
    public SessionManager sessionManager;
    public RLParameters rLParameters;

    public float CurrentCumulativeReward;
    public int CurrentStep;

    private float speedBarrier;
    private float errorPenalty;

    void Start()
    {
        if (rLParameters == null)
        {
            rLParameters = this.GetComponent<RLParameters>();
        }

        if (MaxStep > 0){
            speedBarrier = MaxStep / 4000;
            errorPenalty = - 1 / MaxStep * 4;
        }
    }


    public void FixedUpdate()
    {
        // Show CumReward and current Step
        CurrentCumulativeReward = GetCumulativeReward();
        CurrentStep = StepCount;
    }

    public override void OnEpisodeBegin()
    {
        // Example for resetting scene
        sessionManager.ResetSession();
    }

    public void OnFinishedTrack()
    {
        // Example what to do after finished oneLap
        AddReward(100f);
        sessionManager.NextRound();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(rLParameters.steering_angle);
        sensor.AddObservation(rLParameters.all_wheels_on_track);
        sensor.AddObservation(rLParameters.speed);
        sensor.AddObservation(rLParameters.waypoint_passed);
        sensor.AddObservation(rLParameters.distance_from_center);
        sensor.AddObservation(rLParameters.round_time);
        sensor.AddObservation(rLParameters.distance_to_next_waypoint);
    }

    public override void OnActionReceived(ActionBuffers _action)
    {
        var action = _action.ContinuousActions;
        sessionManager.VehicleAction(new VehicleAction(action[0], action[1], action[2]));
        RewardFunction();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetAxis("Jump");
    }

    private void RewardFunction()
    {
        // TokyoDriftRewardFunction()
        // SpeedyGonzalesRewardFunction();
        AddReward(-0.001f);

        if (rLParameters.waypoint_passed){
            AddReward(1.0f);
            return;
        }

        if (rLParameters.speed < speedBarrier || !rLParameters.all_wheels_on_track){
            

            float distance = rLParameters.distance_to_next_waypoint;
            if (distance > 30){
                AddReward(10f * errorPenalty);
            } else if (distance > 20){
                AddReward(5f * errorPenalty);
            } else if (distance > 10){
                AddReward(errorPenalty);
            } else {
                // almost good
                AddReward(0.1f * errorPenalty);
            }
        }
    }


    private void SpeedyGonzalesRewardFunction(){
        AddReward(-0.001f);

        if (rLParameters.waypoint_passed){
            AddReward(1.0f);
        }

        if (rLParameters.speed < speedBarrier || rLParameters.distance_from_center > 4 || !rLParameters.all_wheels_on_track){
            

            float distance = rLParameters.distance_to_next_waypoint;
            if (distance > 30){
                AddReward(5f * errorPenalty);
            } else if (distance > 20){
                AddReward(4f * errorPenalty);
            } else if (distance > 10){
                AddReward(0.1f * errorPenalty);
            } else {
                AddReward(errorPenalty);
            }
        }
    }    

    private void TokyoDriftReawardFunction(){
        // penalty for each step ==> acceleration
        AddReward(-0.1f);

        // illegally touching grass
        if (!rLParameters.all_wheels_on_track || !rLParameters.approaching_next_waypoint || rLParameters.speed < 0.1f){
            AddReward(-1.0f);
            // callin EndEpisode() may accelerate learning process, however Agent may start cheating
            // EndEpisode();
            return;
        }

        AddReward(0.3f);

        float allignment = rLParameters.alignment_with_next_waypoint;
        float distance = rLParameters.distance_to_next_waypoint;
        if (allignment > 0.1f && distance < 20){
            AddReward(1.0f);
        }
        else if (allignment > 1 && distance < 15){
            AddReward(5.0f);
        }
        else if (allignment > 3 && distance < 10){
            AddReward(1.00f);
        }

        if (rLParameters.more_waypoints_reached){
            AddReward(1000);
        }
    }
}
