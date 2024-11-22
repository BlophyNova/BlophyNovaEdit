using TMPro;
using UnityEngine;

public class LabelItem : MonoBehaviour
{
    public TMP_Text labelName;
    public CloseLabel closeThisLabel;
    public LabelWindow labelWindow;
    public LabelWindowContent labelWindowContent;

    public delegate void OnLabelGetFocus();
    public event OnLabelGetFocus onLabelGetFocus = () => { };
    public delegate void OnLabelLostFocus();
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
