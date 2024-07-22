using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Singleton;

public class YScale : MonoBehaviourSingleton<YScale>
{
    public float yScale = 6;
    public float CurrentYScale
    {
        get
        {
            if (GlobalData.Instance.chartEditData.yScale == 0) return yScale;
            yScale = GlobalData.Instance.chartEditData.yScale;
            return yScale;
        }
    }
    public float GetPositionYWithSecondsTime(float secondsTime)
    {
        float currentTime = secondsTime * 100;
        return currentTime * CurrentYScale;
    }
    private void Start()
    {
    }
}
