using Controller;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.BoxList
{
    public class Add : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() => 
            {
                GlobalData.Instance.chartEditData.boxes.Add(GlobalData.Instance.CreateNewBox());
                GlobalData.Instance.chartData.boxes = GlobalData.Instance.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                GameController.Instance.RefreshChartPreview();
            });
        }
    }
}