using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxID : MonoBehaviour,IRefresh
{
    public TMP_Text thisText;
    public Button add;
    public Button subtraction;
    public PropertyEdit propertyEdit;
    public void Refresh()
    {
        //if((LabelWindowContentType.NoteEdit | LabelWindowContentType.EventEdit).HasFlag(propertyEdit.labelWindow.associateLabelWindow.currentLabelWindow.labelWindowContentType))
        //{
        //}
    }

    private void Start()
    {
        add.onClick.AddListener(() =>
        {
            GlobalData.Instance.chartEditData.boxID++;
            thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.boxID}";
            GlobalData.Refresh();
        });
        subtraction.onClick.AddListener(() =>
        {
            GlobalData.Instance.chartEditData.boxID--;
            thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.boxID}";
            GlobalData.Refresh();
        });
    }
}
