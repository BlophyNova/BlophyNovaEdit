using Log;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class LoopPlayback : MonoBehaviour
    {
        public Toggle isOn;

        private void Start()
        {
            isOn.onValueChanged.AddListener(on =>
            {
                GlobalData.Instance.chartEditData.loopPlayBack = on;
                LogCenter.Log($"属性编辑，循环播放从{!on}变更为{on}");
            });
            isOn.SetIsOnWithoutNotify(GlobalData.Instance.chartEditData.loopPlayBack);
        }
    }
}