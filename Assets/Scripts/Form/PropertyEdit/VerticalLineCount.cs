using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class VerticalLineCount : MonoBehaviour
    {
        public TMP_Text thisText;
        public Button add;
        public Button subtraction;

        private void Start()
        {
            add.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.verticalSubdivision++;
                thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.verticalSubdivision}";

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
                LogCenter.Log($"属性编辑执行+操作，垂直份数更改为{GlobalData.Instance.chartEditData.verticalSubdivision}");
            });
            subtraction.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.verticalSubdivision--;
                thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.verticalSubdivision}";

                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
                LogCenter.Log($"属性编辑执行-操作，垂直份数更改为{GlobalData.Instance.chartEditData.verticalSubdivision}");
            });
            thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.verticalSubdivision}";
        }
    }
}