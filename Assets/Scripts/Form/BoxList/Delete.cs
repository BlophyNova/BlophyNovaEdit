using Controller;
using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UtilityCode.ChartTool;

namespace Form.BoxList
{
    public class Delete : PublicButton
    {
        public BoxListItem boxListItem;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                int boxID = GlobalData.Instance.chartEditData.boxes.FindIndex(m => m == boxListItem.thisBox);
                if (boxID < 0)
                {
                    return;
                }

                LogCenter.Log($"{boxID}号框被删除");
                boxListItem.boxList.boxListItems.Remove(boxListItem);
                GlobalData.Instance.chartEditData.boxes.Remove(boxListItem.thisBox);
                Destroy(boxListItem.gameObject);
                GlobalData.Instance.chartData.boxes =
                    ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(), new() { typeof(BoxList) });
                GameController.Instance.RefreshChartPreview();
            });
        }
    }
}