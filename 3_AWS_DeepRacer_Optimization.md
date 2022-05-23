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

### test (alt)

```python
def reward_function(params):
    # Read input parameters
    track_width = params['track_width']
    distance_from_center = params['distance_from_center']

    # Calculate markers that are at varying distances away from the center line
    marker_0 = 0.05 * track_width
    marker_1 = 0.15 * track_width
    marker_2 = 0.30 * track_width
    marker_3 = 0.5 * track_width

    # Give higher reward if the car is closer to center line and vice versa
    if distance_from_center <= marker_0:
        reward = 1.0
    elif distance_from_center <= marker_1:
        reward = 0.8
    elif distance_from_center <= marker_2:
        reward = 0.42
    elif distance_from_center <= marker_3:
        reward = 0.05
    else:
        reward = 1e-5 # likely crashed/ close to off track

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

### apex-line

```python
def reward_function(params):
    # Read input parameters
    all_wheels_on_track = params["all_wheels_on_track"]
    is_crashed = params["is_crashed"]
    is_offtrack = params["is_offtrack"]
    progress = params['progress']
    steps = params['steps']

    # Check if agent is on track
    if not all_wheels_on_track or is_crashed or is_offtrack:
        reward = 1e-5
    else:
       # reward for shortest racing line
       reward = progress / steps
        

    return float(reward)
```

### ril-speed : test + speed

siehe test und speed

### ril-steering : ril-speed

Bestrafung von Zig-Zack-Linien, zu kleinen Lenkradiien und langsamem Fahren

```python
def reward_function(params):
    # Constants
    SPEED_THRESHOLD = 1.0
    ABS_STEERING_THRESHOLD = 20.0

    # Read input parameters
    track_width = params["track_width"]
    distance_from_center = params["distance_from_center"]
    abs_steering = abs(params["steering_angle"])
    all_wheels_on_track = params["all_wheels_on_track"]
    speed = params["speed"]

    # Calculate markers that are at varying distances away from the center line
    marker_0 = 0.05 * track_width
    marker_1 = 0.15 * track_width
    marker_2 = 0.30 * track_width
    marker_3 = 0.5 * track_width

    # init reward
    reward = 0.0

    # Give higher reward if the car is closer to center line and vice versa
    if distance_from_center <= marker_0:
        reward = 1.0
    elif distance_from_center <= marker_1:
        reward = 0.8
    elif distance_from_center <= marker_2:
        reward = 0.42
    elif distance_from_center <= marker_3:
        reward = 0.05

    if abs_steering > ABS_STEERING_THRESHOLD:
        reward *= 0.8

    if not all_wheels_on_track:
        # Penalize if the car goes off track
        reward = -1.0
    elif speed < SPEED_THRESHOLD:
        # Penalize if the car goes too slow
        reward *= 0.5
    else:
        # High reward if the car stays on track and goes fast
        reward *= 2.0

    return float(reward)
```
### testy : test

```python
def reward_function(params):
    SPEED_THRESHOLD = 1.0

    # Read input parameters
    track_width = params['track_width']
    distance_from_center = params['distance_from_center']
    speed = params["speed"]

    # Calculate markers that are at varying distances away from the center line
    marker_0 = 0.1 * track_width
    marker_1 = 0.2 * track_width
    marker_2 = 0.35 * track_width
    marker_3 = 0.5 * track_width

    # init reward
    reward = 0.0

    # Give higher reward if the car is closer to center line and vice versa
    if distance_from_center <= marker_0:
        reward = 1.0
    elif distance_from_center <= marker_1:
        reward = 0.8
    elif distance_from_center <= marker_2:
        reward = 0.42
    elif distance_from_center <= marker_3:
        reward = 0.05
    else:
        reward = 1e-5 # likely crashed/ close to off track

    if speed < SPEED_THRESHOLD:
        # Penalize if the car goes too slow
        reward *= 0.5
    else:
        reward *= 2.0

    return float(reward)
```

### next-gen-test

```python
def reward_function(params):
    SPEED_THRESHOLD = 1.0

    # Read input parameters
    track_width = params['track_width']
    distance_from_center = params['distance_from_center']
    speed = params["speed"]

    # Calculate markers that are at varying distances away from the center line
    marker_0 = 0.05 * track_width
    marker_1 = 0.20 * track_width
    marker_2 = 0.35 * track_width
    marker_3 = 0.5 * track_width
    
    # init reward
    reward = 0.0

    # Give higher reward if the car is closer to center line and vice versa
    if distance_from_center <= marker_0:
        reward = 1.0
    elif distance_from_center <= marker_1:
        reward = 0.75
    elif distance_from_center <= marker_2:
        reward = 0.42
    elif distance_from_center <= marker_3:
        reward = 0.05
    else:
        reward = -1.0 # likely crashed/ close to off track

    if speed < SPEED_THRESHOLD:
        # Penalize if the car goes too slow
        reward *= 0.8
    else:
        reward *= 2.0

    return float(reward)
```

### Roadrunner

```python
def reward_function(params):
    # Read input variables
    all_wheels_on_track = params["all_wheels_on_track"]
    is_crashed = params["is_crashed"]
    speed = params["speed"]

    if is_crashed or not all_wheels_on_track:
        award = 0.0001
    else:
        award = speed

    return award
```


## Platzierung


| **Modell**   | **Fahrzeit** | **∅-Runde** | **Platzierung** | **Besser als** | **Resets** | 
| ------------ | ------------ | ----------- | --------------- | -------------- | ---------- | 
| test         | 02:55.655    | 00:58.551   |  263 / 1717     | 84,68 %        | 0          |
| Roadrunner   | 02:57.526    | 00:59:262   | 556 / 2547      | 78,17 %        | 0          |
| testy        | 03:00.377    | 01:00.126   |  461 / 1737     | 73,46 %        | 0          |
| ril-speed    | 03:00.996    | 01:00.332   |  485 / 1717     | 71,75 %        | 0          |
| speed        | 03:05.744    | 01:01.915   |  765 / 1717     | 55,44 %        | 1          |
| apex-line    | 03:17.671    | 01:05.890   | 1165 / 1717     | 32,14 %        | 3          |
| ril-steering | 03:22.799    | 01:07.599	| 1248 / 1732     | 27,94 %        | 5          |
| guilia       | 03:25.397    | 01:08.466   | 1268 / 1732     | 26,78 %        | 0          |


## Beispiel Performance Roadrunner

[![AWS Model Roadrunner](https://img.youtube.com/vi/yQd8hDzYgt0/0.jpg)](https://youtu.be/yQd8hDzYgt0)
