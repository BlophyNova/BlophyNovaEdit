using Log;
using UnityEngine.UI;
using UtilityCode.Singleton;

namespace Form.PropertyEdit
{
    public class LoopPlayback : MonoBehaviourSingleton<LoopPlayback>
    {
        public Toggle isOn;

        private void Start()
        {
            isOn.onValueChanged.AddListener(on => { LogCenter.Log($"属性编辑，循环播放从{!on}变更为{on}"); });
        }
    }
}