using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAction
{
    public float Steering { get; private set; }
    public float Throttle { get; private set; }
    public float Brake { get; private set; }

    public VehicleAction(float Steering, float Throttle, float Brake)
    {
        this.Steering = Steering;
        this.Throttle = Throttle;
        this.Brake = Brake;
    }
}
