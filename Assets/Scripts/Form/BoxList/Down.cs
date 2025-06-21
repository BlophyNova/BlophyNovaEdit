using System;
using System.Collections.Generic;
using Controller;
using Data.ChartEdit;
using Data.Interface;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;

namespace Form.BoxList
{
    public class Down : PublicButton
    {
        public BoxListItem boxListItem;
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                int currentBoxIndex = boxListItem.currentBoxIndex;
                
                Box currentBox = GlobalData.Instance.chartEditData.boxes[currentBoxIndex];
                Box lastBox = GlobalData.Instance.chartEditData.boxes[currentBoxIndex+1];
                GlobalData.Instance.chartEditData.boxes[currentBoxIndex] = lastBox;
                GlobalData.Instance.chartEditData.boxes[currentBoxIndex+1] = currentBox;

                Data.ChartData.Box currentChartDataBox = GlobalData.Instance.chartData.boxes[currentBoxIndex];
                Data.ChartData.Box lastChartDataBox = GlobalData.Instance.chartData.boxes[currentBoxIndex+1];
                GlobalData.Instance.chartData.boxes[currentBoxIndex] = lastChartDataBox;
                GlobalData.Instance.chartData.boxes[currentBoxIndex+1] = currentChartDataBox;
                
                Refresh();
            });
        }

        void Refresh()
        {
            //GlobalData.Refresh<IRefreshUI>(m=>m.RefreshUI(),new List<Type>{typeof(BoxList)});
            //GlobalData.Refresh<IRefreshPlayer>(m=>m.RefreshPlayer(-1,-1),null);
            
            GlobalData.Refresh<IRefreshEdit>(m=>m.RefreshEdit(-1,-1),new List<Type>{typeof(NoteEdit.NoteEdit),typeof(EventEdit.EventEdit)});
            GameController.Instance.RefreshChartPreview();
        }
    }
}
