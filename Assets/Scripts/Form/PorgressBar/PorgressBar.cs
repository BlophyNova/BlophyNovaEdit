using System;
using System.Collections.Generic;
using Data.Interface;
using Form.LabelWindow;
using Form.NoteEdit;
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
                float result = GlobalData.Instance.chartData.metaData.musicLength * theValue;
                ProgressManager.Instance.SetTime(result);

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                    new List<Type> { typeof(BasicLine) });
                GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), null);
            });
        }

        private void Update()
        {
            if (GlobalData.Instance.chartData.metaData.musicLength <= 1)
            {
                return;
            }

            float currentProgress = (float)ProgressManager.Instance.CurrentTime /
                                    GlobalData.Instance.chartData.metaData.musicLength;
            progressBar.SetValueWithoutNotify(currentProgress);
            if (currentProgress >= .9999f)
            {
                StateManager.Instance.RestartTime(GlobalData.Instance.chartEditData.loopPlayBack);
                //Debug.LogError($"循环播放做好后用在这里");

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                    new List<Type> { typeof(BasicLine) });
            }

            progressInfomation.text = $"\t{(int)(ProgressManager.Instance.CurrentTime / 60):D2}:" +
                                      $"{(int)(ProgressManager.Instance.CurrentTime - (int)(ProgressManager.Instance.CurrentTime / 60) * 60):D2}:" +
                                      $"{(int)((ProgressManager.Instance.CurrentTime - (int)ProgressManager.Instance.CurrentTime) * 1000):D3} \t/\t " +
                                      $"{(int)(GlobalData.Instance.chartData.metaData.musicLength / 60):D2}:" +
                                      $"{(int)(GlobalData.Instance.chartData.metaData.musicLength - (int)(GlobalData.Instance.chartData.metaData.musicLength / 60) * 60):D2}:" +
                                      $"{(int)((GlobalData.Instance.chartData.metaData.musicLength - (int)GlobalData.Instance.chartData.metaData.musicLength) * 1000):D3}\t当前BPM：" +
                                      $"{BPMManager.Instance.thisCurrentTotalBPM}\t当前Beats：" +
                                      $"{BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime):F3}\t";
        }
    }
}