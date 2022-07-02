using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Track))]
public class TrackWaypoints : MonoBehaviour
{
    public List<Vector3> waypointsPositions = new List<Vector3>();
    public Track track;
    public  GameObject waypointHolder;
    private GameObject waypointsLineGameObject;

    public int WaypointsCount => waypointsPositions.Count;
    public float WaypointsTrackLength { get; private set; } = 0.0f;
    public bool ShowWaypointsTrigger;
    public Material WaypointsDebugMaterial;
    public bool ShowWaypointsLine;

    public void ResetTrackWaypoints()
    {
        foreach(Transform waypointTransform in waypointHolder.transform)
        {
            waypointTransform.gameObject.SetActive(true);
        }
    }

    public void Awake()
    {
        if (this.track == null)
        {
            this.track = this.GetComponent<Track>();
        }

        if (track != null)
        {
            this.track.registerOnFinishedBuild(BuildingWaypoints);
        }
    }

    public void BuildingWaypoints()
    {
        if (track != null)
        {
            //Clear old data and objects
            ClearTrackWaypoints();

            CalculateTrackWaypoints();
            CreateGameobjectTriggerWaypoints();
            CalculateWaypointTrackLength();
            if (ShowWaypointsLine)
            {
                DrawDebugWaypointLine();
            }
        }
    }

    public void ClearTrackWaypoints()
    {
        waypointsPositions.Clear();
        Destroy(waypointsLineGameObject);
        Destroy(waypointHolder);
    }

    public void CalculateTrackWaypoints()
    {
        TrackInfo tiIterator = track.StartLineTrackInfo;
        int trackIndex = 0;
        while (tiIterator != null)
        {
            tiIterator.index = trackIndex++;
            waypointsPositions.Add(tiIterator.trackCenterPosition);
            waypointsPositions.Add(tiIterator.trackEndPosition);

            if (tiIterator.nextTrack == track.StartLineTrackInfo)
            {
                break;
            }

            tiIterator = tiIterator.nextTrack;
        }

        if (track.StartLineTrackInfo == track.FinishLineTrackInfo)
        {
            waypointsPositions.Add(track.StartLineTrackInfo.trackCenterPosition);
        }
        else if (track.StartLineTrackInfo != track.FinishLineTrackInfo && track.FinishLineTrackInfo != null)
        {
            waypointsPositions.Add(track.FinishLineTrackInfo.trackCenterPosition);
        }
    }

    public void CreateGameobjectTriggerWaypoints()
    {
        waypointHolder = new GameObject("WaypointsHolder");
        waypointHolder.transform.parent = this.transform.parent;

        WaypointInfo lastWaypointInfo = null;

        for (int i = 0; i < waypointsPositions.Count; i++)
        {
            GameObject go;
            float distanceToNextWaypoint = 0;
            float distanceToLastWaypoint = 0;

            if (i == waypointsPositions.Count - 1)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.localScale = Vector3.one + Vector3.forward * 11;
                //if(track.IsTrackCircular)
                //{
                //    distanceToNextWaypoint = Vector3.Distance(waypointsPositions[i], waypointsPositions[0]);
                //}
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale *= 12;
                distanceToNextWaypoint = Vector3.Distance(waypointsPositions[i], waypointsPositions[i + 1]);
            }

            if (WaypointsDebugMaterial)
            {
                go.GetComponent<MeshRenderer>().material = WaypointsDebugMaterial;
            }
            if (!ShowWaypointsTrigger)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.enabled = false;
                }
            }

            WaypointInfo wi = go.AddComponent<WaypointInfo>();
            wi.index = i;
            wi.distanceToNextWaypoint = distanceToNextWaypoint;

            if(lastWaypointInfo != null)
            {
                distanceToLastWaypoint = lastWaypointInfo.distanceToNextWaypoint;
            }

            wi.distanceToLastWaypoint = distanceToLastWaypoint;

            go.transform.position = waypointsPositions[i];
            go.GetComponent<Collider>().isTrigger = true;
            go.name = "Waypoint_" + i;
            go.tag = "Waypoint";
            go.transform.parent = waypointHolder.transform;

            lastWaypointInfo = wi;
        }
    }

    public void DrawDebugWaypointLine()
    {
        waypointsLineGameObject = new GameObject("Debug_Waypoint_Line");
        waypointsLineGameObject.transform.parent = this.transform;

        LineRenderer lineRenderer = waypointsLineGameObject.AddComponent<LineRenderer>();

        List<Vector3> lineRendererPoints = new List<Vector3>();

        for (int i = 0; i < waypointsPositions.Count; i++)
        {
            if (track.IsTrackCircular && i == waypointsPositions.Count - 1)
            {
                lineRendererPoints.Add(waypointsPositions[i]);
                lineRendererPoints.Add(waypointsPositions[0]);
                //Debug.DrawLine(waypointsPositions[i], waypointsPositions[0], Color.cyan, 1000);
            }
            else
            {
                lineRendererPoints.Add(waypointsPositions[i]);
                lineRendererPoints.Add(waypointsPositions[i+1]);
                //Debug.DrawLine(waypointsPositions[i], waypointsPositions[i + 1], Color.cyan, 1000);
            }
        }

        lineRenderer.positionCount = lineRendererPoints.Count;
        lineRenderer.SetPositions(lineRendererPoints.ToArray());
    }

    public void CalculateWaypointTrackLength()
    {
        for (int i = 0; i < waypointsPositions.Count; i++)
        {
            if (track.IsTrackCircular && i == (waypointsPositions.Count - 1))
            {
                WaypointsTrackLength += Vector3.Distance(waypointsPositions[i], waypointsPositions[0]);
            }
            else
            {
                WaypointsTrackLength += Vector3.Distance(waypointsPositions[i], waypointsPositions[i + 1]);
            }
        }
    }
}
