using Scenes.DontDestoryOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Add : PublicButton
{
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            GlobalData.Instance.chartEditData.bpmList.Add(new(GlobalData.Instance.chartEditData.bpmList[^1]));
            GlobalData.Refresh();
        });
    }
}
