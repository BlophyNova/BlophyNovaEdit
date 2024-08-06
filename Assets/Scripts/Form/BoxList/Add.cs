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
                GlobalData.Instance.chartEditData.boxes.Add(ChartTool.CreateNewBox(GlobalData.Instance.easeData));
                GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                GameController.Instance.RefreshChartPreview();
            });
        }
    }
}