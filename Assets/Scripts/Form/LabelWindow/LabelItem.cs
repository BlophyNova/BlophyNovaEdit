using Log;
using TMPro;
using UnityEngine;

namespace Form.LabelWindow
{
    public class LabelItem : MonoBehaviour
    {
        public delegate void OnLabelGetFocus();

        public delegate void OnLabelLostFocus();

        public TMP_Text labelName;
        public CloseLabel closeThisLabel;
        public LabelButton labelButton;
        public LabelWindow labelWindow;
        public LabelWindowContent labelWindowContent;
        public event OnLabelGetFocus onLabelGetFocus = () => { };
        public event OnLabelLostFocus onLabelLostFocus = () => { };

        public void LabelGetFocus()
        {
            onLabelGetFocus();
            LogCenter.Log($"{labelWindow.labelColorIndex}号窗口{labelWindowContent.labelWindowContentType}标签被激活");
        }

        public void LabelLostFocus()
        {
            onLabelLostFocus();
            LogCenter.Log($"{labelWindow.labelColorIndex}号窗口{labelWindowContent.labelWindowContentType}标签失活");
        }
    }
}