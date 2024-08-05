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
                boxListItem.boxList.boxListItems.Remove(boxListItem);
                GlobalData.Instance.chartEditData.boxes.Remove(boxListItem.thisBox);
                Destroy(boxListItem.gameObject);
                GlobalData.Instance.chartData.boxes = GlobalData.Instance.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                GameController.Instance.RefreshChartPreview();
            });
        }
    }
}
