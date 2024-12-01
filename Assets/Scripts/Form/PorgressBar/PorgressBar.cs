using Data.Interface;
using Form.LabelWindow;
using Form.PropertyEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine.UI;

namespace Form.PorgressBar
{
    public class PorgressBar : LabelWindowContent
    {
        public Scrollbar progressBar;
        public TMP_Text progressInfomation;
        public Button back2Bottom;
        public Button keepBack;
        public Button back3Seconds;
        public Button pauseContinue;
        public Button incalzando3Seconds;
        public Button KeepIncalzando;
        public Button incalzando2End;

        private void Start()
        {
            progressBar.onValueChanged.AddListener(theValue =>
            {
                float result = GlobalData.Instance.chartData.globalData.musicLength * theValue;
                ProgressManager.Instance.SetTime(result);

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            });
        }

        private void Update()
        {
            float currentProgress = (float)ProgressManager.Instance.CurrentTime /
                                    GlobalData.Instance.chartData.globalData.musicLength;
            progressBar.SetValueWithoutNotify(currentProgress);
            if (currentProgress >= .9999f)
            {
                StateManager.Instance.RestartTime(LoopPlayback.Instance.isOn);
                //Debug.LogError($"循环播放做好后用在这里");

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            }

            progressInfomation.text = $"\t{(int)(ProgressManager.Instance.CurrentTime / 60):D2}:" +
                                      $"{(int)(ProgressManager.Instance.CurrentTime - (int)(ProgressManager.Instance.CurrentTime / 60) * 60):D2}:" +
                                      $"{(int)((ProgressManager.Instance.CurrentTime - (int)ProgressManager.Instance.CurrentTime) * 1000):D3} \t/\t " +
                                      $"{(int)(GlobalData.Instance.chartData.globalData.musicLength / 60):D2}:" +
                                      $"{(int)(GlobalData.Instance.chartData.globalData.musicLength - (int)(GlobalData.Instance.chartData.globalData.musicLength / 60) * 60):D2}:" +
                                      $"{(int)((GlobalData.Instance.chartData.globalData.musicLength - (int)GlobalData.Instance.chartData.globalData.musicLength) * 1000):D3}\t当前BPM：" +
                                      $"{BPMManager.Instance.thisCurrentTotalBPM}\t当前Beats：" +
                                      $"{BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime):F3}\t";
        }
    }
}