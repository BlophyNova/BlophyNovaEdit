using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class PlaySpeed : MonoBehaviour
    {
        public TMP_InputField inputField;
        public Button ok;

        private void Start()
        {
            ok.onClick.AddListener(() =>
            {
                if (!float.TryParse(inputField.text, out float playSpeed))
                {
                    return;
                }

                GlobalData.Instance.chartEditData.playSpeed = playSpeed;
                double currentTime = ProgressManager.Instance.CurrentTime;
                ProgressManager.Instance.SetPlaySpeed(GlobalData.Instance.chartEditData.playSpeed);
                ProgressManager.Instance.SetTime(currentTime);

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());

                LogCenter.Log($"属性编辑，播放速度变更为{playSpeed}");
            });
        }
    }
}