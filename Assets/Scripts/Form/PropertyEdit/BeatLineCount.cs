using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class BeatLineCount : MonoBehaviour
    {
        public TMP_Text thisText;
        public Button add;
        public Button subtraction;

        private void Start()
        {
            add.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.beatSubdivision++;
                thisText.text = $"水平线：{GlobalData.Instance.chartEditData.beatSubdivision}";

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
                LogCenter.Log($"属性编辑执行+操作，水平份数更改为{GlobalData.Instance.chartEditData.beatSubdivision}");
            });
            subtraction.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.beatSubdivision--;
                thisText.text = $"水平线：{GlobalData.Instance.chartEditData.beatSubdivision}";

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
                LogCenter.Log($"属性编辑执行-操作，水平份数更改为{GlobalData.Instance.chartEditData.beatSubdivision}");
            });
        }
    }
}