using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;

public class LoopPlayback : MonoBehaviourSingleton<LoopPlayback>
{
    public Toggle isOn;

    private void Start()
    {
        isOn.onValueChanged.AddListener(on =>
        {
            LogCenter.Log($"属性编辑，循环播放从{!on}变更为{on}");
        });
    }
}
