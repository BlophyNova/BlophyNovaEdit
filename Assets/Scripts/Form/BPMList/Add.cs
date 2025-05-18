using System;
using System.Collections.Generic;
using Data.ChartEdit;
using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;

namespace Form.BPMList
{
    public class Add : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                BPM newBpm = new(GlobalData.Instance.chartEditData.bpmList[^1]);
                GlobalData.Instance.chartEditData.bpmList.Add(newBpm);
                LogCenter.Log($"新增了一个BPMItem，{newBpm.integer}:{newBpm.molecule}/{newBpm.denominator}");

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                    new List<Type> { typeof(BPMList) });
            });
        }
    }
}