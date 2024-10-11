using Controller;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.BoxList
{
    public class Delete : PublicButton
    {
        public BoxListItem boxListItem;
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                int boxID= GlobalData.Instance.chartEditData.boxes.FindIndex(m => m == boxListItem.thisBox);
                if (boxID < 0) return;
                LogCenter.Log($"{boxID}号框被删除");
                boxListItem.boxList.boxListItems.Remove(boxListItem);
                GlobalData.Instance.chartEditData.boxes.Remove(boxListItem.thisBox);
                Destroy(boxListItem.gameObject);
                GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                GameController.Instance.RefreshChartPreview();
            });
        }
    }
}
