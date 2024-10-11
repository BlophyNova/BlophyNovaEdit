using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWindow : PublicButton
{
    public LabelWindow labelWindow;
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            //labelWindow.gameObject.SetActive(false);
            LabelWindowsManager.Instance.SetUsedColor2Unused(labelWindow.labelColorIndex);
            LabelWindowsManager.Instance.windows.Remove(labelWindow);
            LogCenter.Log($"{labelWindow.labelColorIndex}号窗口被销毁");
            Destroy(labelWindow.gameObject);
        });
    }
}
