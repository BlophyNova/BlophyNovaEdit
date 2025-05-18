using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class PlaySpeed : MonoBehaviour
    {
        public TMP_Dropdown speedOptions;
        private readonly Dictionary<int, float> option2Speed = new() 
        {
            { 0, 1 },
            { 1, .75f },
            { 2, .5f },
            { 3, .25f },
            { 4, .125f },
            { 5, 1.25f },
            { 6, 1.5f },
            { 7, 2 },
            { 8, 3 },
            { 9, 5 },
        };

        private void Start()
        {
            speedOptions.onValueChanged.AddListener(v =>
            {
                if (!option2Speed.TryGetValue(v, out float speed)) return;
                GlobalData.Instance.chartEditData.playSpeed = speed;
                double currentTime = ProgressManager.Instance.CurrentTime;
                ProgressManager.Instance.SetPlaySpeed(GlobalData.Instance.chartEditData.playSpeed);
                ProgressManager.Instance.SetTime(currentTime);

                LogCenter.Log($"属性编辑，播放速度变更为{speed}");
            });
        }
    }
}