# AWS DeepRacer Student

<a href="https://student.deepracer.com">
   <img src="./res/1_AWS_Logo.png" width="200" height="200"/>
</a>

## Input Parameter

Die Paramter für die Reward-Funcion und ihre Erklärung kann [hier](https://docs.aws.amazon.com/deepracer/latest/developerguide/deepracer-reward-function-input.html) nachgelesen werden.

```python
{
    "all_wheels_on_track": Boolean,        # flag to indicate if the agent is on the track
    "x": float,                            # agent's x-coordinate in meters
    "y": float,                            # agent's y-coordinate in meters
    "closest_objects": [int, int],         # zero-based indices of the two closest objects to the agent's current position of (x, y).
    "closest_waypoints": [int, int],       # indices of the two nearest waypoints.
    "distance_from_center": float,         # distance in meters from the track center 
    "is_crashed": Boolean,                 # Boolean flag to indicate whether the agent has crashed.
    "is_left_of_center": Boolean,          # Flag to indicate if the agent is on the left side to the track center or not. 
    "is_offtrack": Boolean,                # Boolean flag to indicate whether the agent has gone off track.
    "is_reversed": Boolean,                # flag to indicate if the agent is driving clockwise (True) or counter clockwise (False).
    "heading": float,                      # agent's yaw in degrees
    "objects_distance": [float, ],         # list of the objects' distances in meters between 0 and track_length in relation to the starting line.
    "objects_heading": [float, ],          # list of the objects' headings in degrees between -180 and 180.
    "objects_left_of_center": [Boolean, ], # list of Boolean flags indicating whether elements' objects are left of the center (True) or not (False).
    "objects_location": [(float, float),], # list of object locations [(x,y), ...].
    "objects_speed": [float, ],            # list of the objects' speeds in meters per second.
    "progress": float,                     # percentage of track completed
    "speed": float,                        # agent's speed in meters per second (m/s)
    "steering_angle": float,               # agent's steering angle in degrees
    "steps": int,                          # number steps completed
    "track_length": float,                 # track length in meters.
    "track_width": float,                  # width of the track
    "waypoints": [(float, float), ]        # list of (x,y) as milestones along the track center
}
```
Alle Modelle haben folgende Gemeinsamkeiten:
- `Ace Speedway` als Strecke,
- 60 Minuten initiale Trainigszeit und
- `Proximal Policy Optimization` als Optimierer

Unterschiede gibt es in der `reward_function`

## Modelle 

### test (alt)

```python
def reward_function(params):
   # Read input parameters
   track_width = params['track_width']
   distance_from_center = params['distance_from_center']

   # Calculate markers that are at varying distances away from the center line
   marker_0 = 0.05 * track_width
   marker_1 = 0.15 * track_width
   marker_2= 0.30 * track_width
   marker_3 = 0.5 * track_width

   # Give higher reward if the car is closer to center line and vice versa
   if distance_from_center <= marker_0:
      reward =1.0
   elif distance_from_center <= marker_1:
      reward =0.8
   elif distance_from_center <= marker_2:
      reward = 0.42
   elif distance_from_center <= marker_3:
      reward =0.05
   else:
      reward = 1e-5 # likely crashed/ close to off track

   return float(reward)
```

### guilia (alt)

```python
def reward_function(params):
   # Read input parameters
   all_wheels_on_track = params['all_wheels_on_track']
   distance_from_center = params['distance_from_center']
   track_width = params['track_width']
   
   # Give a high reward if no wheels go off the track and
   # the agent is somewhere in between the track borders
   if all_wheels_on_track and (0.5*track_width - distance_from_center) >= 0.05:
      reward = 1.0
   else
      reward = 1e-3
   
   return float(reward)
```

### speed
```python
def reward_function(params):
    # Read input parameters
    speed = params["speed"]
    all_wheels_on_track = params["all_wheels_on_track"]
    is_crashed = params["is_crashed"]
    is_offtrack = params["is_offtrack"]

    # Check if agent is on track
    if not all_wheels_on_track or is_crashed or is_offtrack:
        reward = 1e-5
    else:
        MAX_SPEED = 4
        speed_rate = speed / MAX_SPEED
        # square reward for high learning impact if agent is fast
        reward = speed_rate ** 2

    return float(reward)
```



Platzierung richtet sich nach der Rangliste am 14.05.2022

| **Modell** | **Fahrzeit** | **∅-Runde** | **Platzierung** | **Resets** |
| ---------- | ------------ | ----------- | --------------- | ---------- | 
| test       | 02:55.655    | 00:58.551   | 261/1698        | 0          |
| speed      | 03:05.744    | 02:04.660   | 1664/1698       | 1          |
| guilia     | 03:25.397    | 01:08.466   | 1236/1698       | 0          |
