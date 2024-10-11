using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;

public class YScale : MonoBehaviourSingleton<YScale>
{
    public TMP_InputField inputField;
    public Button ok;
    public float yScale = 6;
    private void Start()
    {
        ok.onClick.AddListener(() => 
        {
            if (!float.TryParse(inputField.text, out float yScale)) return;
            
            LogCenter.Log($"属性编辑，Y轴缩放从{CurrentYScale}变更为{yScale}");
            CurrentYScale = yScale;
            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
    }
    public float CurrentYScale
    {
        get
        {
            if (GlobalData.Instance.chartEditData.yScale == 0) return yScale;
            yScale = GlobalData.Instance.chartEditData.yScale;
            return yScale;
        }
        private set => GlobalData.Instance.chartEditData.yScale = value;
        
    }
    public float GetPositionYWithSecondsTime(float secondsTime)
    {
        float currentTime = secondsTime * 100;
        return currentTime * CurrentYScale;
    }
    public float GetPositionYWithBeats(float beats)
    {
        return GetPositionYWithSecondsTime(BPMManager.Instance.GetSecondsTimeWithBeats(beats));
    }
}
