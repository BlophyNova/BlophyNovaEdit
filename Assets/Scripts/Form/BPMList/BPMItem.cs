using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Data.ChartEdit;
using Data.Interface;
using Form.NoteEdit;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.BPMList
{
    public class BPMItem : MonoBehaviour
    {
        public BPM myBPM;
        public TMP_InputField bpmValue;
        public TMP_InputField startBeats;
        public Button deleteBPM;

        private void Start()
        {
            bpmValue.onEndEdit.AddListener(v =>
            {
                if (float.TryParse(bpmValue.text, out float result))
                {
                    LogCenter.Log(
                        $"{myBPM.integer}:{myBPM.molecule}/{myBPM.denominator}Value被修改，原始值：{myBPM.currentBPM},修改后的值：{result}");
                    myBPM.currentBPM = result;
                    BPMManager.UpdateInfo(GlobalData.Instance.chartEditData.bpmList);
                }

                //GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            });
            startBeats.onEndEdit.AddListener(v =>
            {
                Match match = Regex.Match(v, @"(\d+):(\d+)/(\d+)");
                if (match.Success)
                {
                    LogCenter.Log(
                        $"{myBPM.integer}:{myBPM.molecule}/{myBPM.denominator}Beat被修改，原始值：{myBPM.integer}:{myBPM.molecule}/{myBPM.denominator},修改后的值：{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                    myBPM.integer = int.Parse(match.Groups[1].Value);
                    myBPM.molecule = int.Parse(match.Groups[2].Value);
                    myBPM.denominator = int.Parse(match.Groups[3].Value);
                    BPMManager.UpdateInfo(GlobalData.Instance.chartEditData.bpmList);

                    GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),
                        new List<Type> { typeof(BasicLine) });
                }
            });
            deleteBPM.onClick.AddListener(() =>
            {
                if (BPMManager.Instance.bpmList.Count > 1)
                {
                    BPMManager.Instance.bpmList.Remove(myBPM);
                    BPMManager.UpdateInfo(GlobalData.Instance.chartEditData.bpmList);
                    LogCenter.Log($"{myBPM.integer}:{myBPM.molecule}/{myBPM.denominator}被删除");

                    GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                        new List<Type> { typeof(BPMList), typeof(BasicLine) });
                    Destroy(gameObject);
                }
                else
                {
                    Alert.EnableAlert("呜呜呜，咱保留至少一个bpm可以嘛？");
                }
            });
        }
    }
}