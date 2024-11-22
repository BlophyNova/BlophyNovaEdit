using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FixNoneAudio : PublicButton
{
    // Start is called before the first frame update
    void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            double currentTime = ProgressManager.Instance.CurrentTime;
            ProgressManager.Instance.ResetTime();
            ProgressManager.Instance.StartPlay();
            ProgressManager.Instance.Offset = GlobalData.Instance.chartEditData.offset;
            ProgressManager.Instance.SetTime(currentTime);
            LogCenter.Log("成功执行修复无声操作");
        });
    }
}
