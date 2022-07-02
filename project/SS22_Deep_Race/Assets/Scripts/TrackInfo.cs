using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackInfo : MonoBehaviour
{
    public GameObject Track;

    public enum TrackType {Straight, Curve}
    public enum TrackEvent { Nothing, Start, Finish, Start_And_Finish}


    public int index = -1;

    public TrackInfo nextTrack;
    public TrackInfo previousTrack;

    //[HideInInspector]
    public TrackInfo trackStartLink;
    //[HideInInspector]
    public TrackInfo trackEndLink;

    [Space(16)]
    public TrackType trackType;


    public TrackEvent trackEvent;
    [Space(16)]
    public GameObject trackStart;
    public GameObject trackEnd;


    [Header("TrackType: Curve")]
    public GameObject curveCenterPoint;


    public float trackWidth;
    public Vector3 trackStartPosition;
    public Vector3 trackEndPosition;
    public Vector3 trackCenterPosition;

    public Vector3 trackCurveCenterPointPosition;

    //public Vector3 startFinishLinePosition;

    public float waypointTrackLength;

    // Start is called before the first frame update
    void Awake()
    {


        this.name = this.name + "_" + this.gameObject.transform.parent.name;

        if(Track != null)
        {
            trackCenterPosition = this.Track.transform.position;

            if (trackStart != null)
            {
                BoxCollider bc = trackStart.GetComponent<BoxCollider>();
                trackStartPosition = bc.bounds.center;
                trackWidth = bc.size.z;

                waypointTrackLength += Vector3.Distance(trackStartPosition, trackCenterPosition);
            }
            if (trackEnd != null)
            {
                BoxCollider bc = trackEnd.GetComponent<BoxCollider>();
                trackEndPosition = bc.bounds.center;
                trackWidth = bc.size.z;

                waypointTrackLength += Vector3.Distance(trackEndPosition, trackCenterPosition);
            }

            if(curveCenterPoint != null)
            {
                trackCurveCenterPointPosition = curveCenterPoint.transform.position;
            }
        }



        //if(startFinishLine != null)
        //{
        //    BoxCollider bc = startFinishLine.GetComponent<BoxCollider>();
        //    startFinishLinePosition = bc.bounds.center;
        //}

        

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
