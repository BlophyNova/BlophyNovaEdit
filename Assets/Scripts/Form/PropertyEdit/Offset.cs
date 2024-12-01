using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class Offset : MonoBehaviour
    {
        public TMP_InputField inputField;
        public Button ok;

        private void Start()
        {
            inputField.text = $"{GlobalData.Instance.chartEditData.offset * 1000f}";
            ok.onClick.AddListener(() =>
            {
                if (float.TryParse(inputField.text, out float result))
                {
                    GlobalData.Instance.chartEditData.offset = result / 1000f;
                    WebManager.Instance.RefreshChartData();
                    ProgressManager.Instance.Offset = GlobalData.Instance.chartEditData.offset;
                    LogCenter.Log($"属性编辑，偏移变更为{result}");
                }
            });
        }
    }
}