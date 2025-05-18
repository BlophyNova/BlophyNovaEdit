using Data.Interface;
using Form.NoteEdit;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
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

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(), new() { typeof(BasicLine) });
                LogCenter.Log($"属性编辑执行+操作，水平份数更改为{GlobalData.Instance.chartEditData.beatSubdivision}");
            });
            subtraction.onClick.AddListener(() =>
            {
                if (GlobalData.Instance.chartEditData.beatSubdivision - 1 < 1)
                {
                    Alert.EnableAlert("已经减到最低了呜呜呜···");
                    return;
                }
                GlobalData.Instance.chartEditData.beatSubdivision--;
                thisText.text = $"水平线：{GlobalData.Instance.chartEditData.beatSubdivision}";

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(), new() { typeof(BasicLine) });
                LogCenter.Log($"属性编辑执行-操作，水平份数更改为{GlobalData.Instance.chartEditData.beatSubdivision}");
            });
            thisText.text = $"水平线：{GlobalData.Instance.chartEditData.beatSubdivision}";
        }
    }
}