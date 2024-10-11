using Controller;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshUI : PublicButton
{
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        { 
            CameraController.Instance.CameraAreaUpdate();
            GlobalData.Refresh<IRefreshUI>((interfaceMethod)=>interfaceMethod.RefreshUI());
            LogCenter.Log("成功刷新制谱器全局UI适配");
        });
    }
}
