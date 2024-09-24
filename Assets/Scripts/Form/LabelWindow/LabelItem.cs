using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void LabelGetFocus() => onLabelGetFocus();
    public void LabelLostFocus() => onLabelLostFocus();
}
