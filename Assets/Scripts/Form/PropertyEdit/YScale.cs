using System;
using System.Collections.Generic;
using Data.Interface;
using Form.NoteEdit;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine.UI;
using UtilityCode.Singleton;

namespace Form.PropertyEdit
{
    public class YScale : MonoBehaviourSingleton<YScale>
    {
        public TMP_InputField inputField;
        public Button ok;
        public float yScale = 6;

        public float CurrentYScale
        {
            get
            {
                if (GlobalData.Instance.chartEditData.yScale == 0)
                {
                    return yScale;
                }

                yScale = GlobalData.Instance.chartEditData.yScale;
                return yScale;
            }
            private set => GlobalData.Instance.chartEditData.yScale = value;
        }

        private void Start()
        {
            ok.onClick.AddListener(() =>
            {
                if (!float.TryParse(inputField.text, out float yScale))
                {
                    return;
                }

                LogCenter.Log($"属性编辑，Y轴缩放从{CurrentYScale}变更为{yScale}");
                CurrentYScale = yScale;
                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                    new List<Type> { typeof(BasicLine) });
                GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),
                    new List<Type> { typeof(EventEdit.EventEdit), typeof(NoteEdit.NoteEdit) });
            });
        }

        public float GetPositionYWithSecondsTime(float secondsTime)
        {
            float currentTime = secondsTime * 100;
            return currentTime * CurrentYScale;
        }

        public float GetPositionYWithBeats(float beats)
        {
            return GetPositionYWithSecondsTime(BPMManager.Instance.GetSecondsTimeByBeats(beats));
        }
    }
}