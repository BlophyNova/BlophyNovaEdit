using Form.NoteEdit;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxID : MonoBehaviour,IRefresh
{
    public int boxID;
    public TMP_Text thisText;
    public Button add;
    public Button subtraction;
    public PropertyEdit propertyEdit;

    public void Refresh()
    {
    }

    public void RefreshNote()
    {
        if (propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.EventEdit)
        {
            EventEdit eventEdit = (EventEdit)propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            eventEdit.RefreshEvents(boxID);
        }
        if (propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            NoteEdit noteEdit = (NoteEdit)propertyEdit.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            noteEdit.RefreshNotes(boxID,-1);
        }
    }

    private void Start()
    {
        add.onClick.AddListener(() =>
        {
            if(boxID + 1>= GlobalData.Instance.chartEditData.boxes.Count)
            {
                Alert.EnableAlert("呜呜呜，前方好像是不存在的区域呢...");
                return;
            }
            boxID++;
            thisText.text = $"框号：{boxID}";
            RefreshNote();
            LogCenter.Log($"属性编辑执行+操作，框号更改为{boxID}");
        });
        subtraction.onClick.AddListener(() =>
        {
            if (boxID - 1 < 0)
            {
                Alert.EnableAlert("呜呜呜，前方好像是不存在的区域呢...");
                return;
            }
            boxID--;
            thisText.text = $"框号：{boxID}";
            RefreshNote();
            LogCenter.Log($"属性编辑执行-操作，框号更改为{boxID}");
        });
    }
}
