using Form.NoteEdit;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineID : MonoBehaviour,IRefresh
{
    public int lineID;
    public TMP_Text thisText;
    public Button add;
    public Button subtraction;
    public PropertyEdit propertyEdit;
    public void Refresh()
    {

    }

    private void Start()
    {
        add.onClick.AddListener(() =>
        {
            //if (lineID + 1 > 4)
            if (lineID + 1 > 3)
            {
                Alert.EnableAlert("呜呜呜，前方好像是不存在的区域呢...");
                return;
            }
            lineID++;
            thisText.text = $"线号：{lineID}";

            RefreshNote();
            LogCenter.Log($"属性编辑执行+操作，线号更改为{lineID}");
        });
        subtraction.onClick.AddListener(() =>
        {
            if (lineID - 1 < 0)
            {
                Alert.EnableAlert("呜呜呜，前方好像是不存在的区域呢...");
                return;
            }
            lineID--;
            thisText.text = $"线号：{lineID}";

            RefreshNote();
            LogCenter.Log($"属性编辑执行-操作，线号更改为{lineID}");
        });
    }

    private void RefreshNote()
    {
        if (propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            NoteEdit noteEdit = (NoteEdit)propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            noteEdit.RefreshNotes(-1, lineID);
        }
    }
}
