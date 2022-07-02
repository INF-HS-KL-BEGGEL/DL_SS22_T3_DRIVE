using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Track : MonoBehaviour
{
    public UnityAction OnFinishedBuild;

    public enum TrackDirection { Normal, Mirrored }
    public TrackDirection trackDirection;

    public TrackInfo[] trackInfoList;

    public TrackInfo StartLineTrackInfo;
    public TrackInfo FinishLineTrackInfo;
    public bool IsTrackValid;
    public bool IsTrackCircular;
    public bool IsTrackMirrored;
    public int TrackSize;

    public float ThresholdDistanceForEqual = 0.5f;


    void Start()
    {
        trackInfoList = this.GetComponentsInChildren<TrackInfo>();
        this.IsTrackMirrored = trackDirection == TrackDirection.Mirrored;

        BuildTrack();
    }

    public void registerOnFinishedBuild(UnityAction callback)
    {
        OnFinishedBuild += callback;
    }

    public void BuildTrack()
    {
        FindStartAndOrFinish();

        if(IsTrackValid)
        {
            FindLinksViaThresholdDistance();

            SortTrackByDirection();

            OnFinishedBuild.Invoke();
        }
    }

    public void FindStartAndOrFinish()
    {
        int countStart = 0;
        int countFinish = 0;

        foreach (TrackInfo trackInfo in trackInfoList)
        {
            if (trackInfo.trackEvent == TrackInfo.TrackEvent.Start)
            {
                countStart++;
                StartLineTrackInfo = trackInfo;
            }
            if (trackInfo.trackEvent == TrackInfo.TrackEvent.Finish)
            {
                countFinish++;
                FinishLineTrackInfo = trackInfo;
            }
            if (trackInfo.trackEvent == TrackInfo.TrackEvent.Start_And_Finish)
            {
                countStart++;
                countFinish++;
                StartLineTrackInfo = trackInfo;
                FinishLineTrackInfo = trackInfo;
            }
        }

        string errorMessage = null;

        if (countStart == 0)
        {
            errorMessage = "No start line found!";
        }
        if (countFinish == 0)
        {
            errorMessage = "No finish line found!";
        }
        if (countStart > 1)
        {
            errorMessage = countStart + " start lines found!";
        }
        if (countFinish > 1)
        {
            errorMessage = countFinish + " finish lines found!";
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            IsTrackValid = true;
        }
        else
        {
            Debug.LogError(errorMessage);
            IsTrackValid = false;
        }
    }

    public void FindLinksViaThresholdDistance()
    {
        List<TrackInfo> tmpRegisteredTrackInfos = new List<TrackInfo>(trackInfoList);

        foreach (TrackInfo ti in tmpRegisteredTrackInfos.Reverse<TrackInfo>())
        {

            foreach (TrackInfo ti2 in tmpRegisteredTrackInfos.Reverse<TrackInfo>())
            {
                if (ti == ti2)
                    continue;

                if (Vector3.Distance(ti.trackStartPosition, ti2.trackEndPosition) < ThresholdDistanceForEqual)
                {
                    ti.trackStartLink = ti2;
                    ti2.trackEndLink = ti;
                }
                if (Vector3.Distance(ti.trackStartPosition, ti2.trackStartPosition) < ThresholdDistanceForEqual)
                {
                    ti.trackStartLink = ti2;
                    ti2.trackStartLink = ti;
                }
                if (Vector3.Distance(ti.trackEndPosition, ti2.trackStartPosition) < ThresholdDistanceForEqual)
                {
                    ti.trackEndLink = ti2;
                    ti2.trackStartLink = ti;
                }
                if (Vector3.Distance(ti.trackEndPosition, ti2.trackEndPosition) < ThresholdDistanceForEqual)
                {
                    ti.trackEndLink = ti2;
                    ti2.trackEndLink = ti;
                }

                if (ti.trackStartLink != null && ti.trackEndLink != null)
                {
                    tmpRegisteredTrackInfos.Remove(ti);
                }

                if (ti2.trackStartLink != null && ti2.trackEndLink != null)
                {
                    tmpRegisteredTrackInfos.Remove(ti2);
                }
            }
        }
    }

    public void SortTrackByDirection()
    {
        TrackInfo current = StartLineTrackInfo;
        TrackInfo next = StartLineTrackInfo.trackEndLink;
        if (IsTrackMirrored)
        {

            TrackInfo tmpTEL = current.trackEndLink;
            current.trackEndLink = current.trackStartLink;
            current.trackStartLink = tmpTEL;

            Vector3 tmpTEP = current.trackEndPosition;
            current.trackEndPosition = current.trackStartPosition;
            current.trackStartPosition = tmpTEP;
        }



        while (next != null)
        {
            TrackSize++;
            current.nextTrack = next;
            next.previousTrack = current;

            current = next;
            if (current.trackStartLink == current.previousTrack)
            {
                next = current.trackEndLink;
            }
            else
            {
                next = current.trackStartLink;

                TrackInfo tmpTEL = current.trackEndLink;
                current.trackEndLink = current.trackStartLink;
                current.trackStartLink = tmpTEL;

                Vector3 tmpTEP = current.trackEndPosition;
                current.trackEndPosition = current.trackStartPosition;
                current.trackStartPosition = tmpTEP;
            }

            if (current == StartLineTrackInfo)
            {
                IsTrackCircular = true;
                break;
            }
            if (current == FinishLineTrackInfo)
            {
                break;
            }
        }        
    }


}
