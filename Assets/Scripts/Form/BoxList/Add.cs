using Controller;
using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UtilityCode.ChartTool;

namespace Form.BoxList
{
    public class Add : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.boxes.Add(ChartTool.CreateNewBox(GlobalData.Instance.easeDatas));
                GlobalData.Instance.chartData.boxes =
                    ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(), new() { typeof(BoxList)});
                GameController.Instance.RefreshChartPreview();
                LogCenter.Log($"添加了一个新方框，框号为：{GlobalData.Instance.chartEditData.boxes.Count - 1}");
            });
        }
    }
}