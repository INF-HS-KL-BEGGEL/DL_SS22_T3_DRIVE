using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIStatsTracker : MonoBehaviour
{
    public TMP_Text timeTextUI;
    public TMP_Text lapTextUI;


    private string formatSeconds(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
    }

    // Update is called once per frame
    public void SetTextTime(float seconds, int lap)
    {
        if(timeTextUI != null)
        {
            timeTextUI.text = formatSeconds(seconds);
        }

        if(lapTextUI != null)
        {
            lapTextUI.text = lap.ToString();
        }
    }
}
