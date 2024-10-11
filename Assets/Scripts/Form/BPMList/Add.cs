using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using Data.ChartEdit;
using UnityEngine;

public class Add : PublicButton
{
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            BPM newBpm = new(GlobalData.Instance.chartEditData.bpmList[^1]);
            GlobalData.Instance.chartEditData.bpmList.Add(newBpm);
            LogCenter.Log($"新增了一个BPMItem，{newBpm.integer}:{newBpm.molecule}/{newBpm.denominator}");

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
    }
}
