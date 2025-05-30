using System;
using System.Collections.Generic;
using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class LineID : MonoBehaviour, IRefreshEdit
    {
        public int lineID;
        public TMP_Text thisText;
        public Button add;
        public Button subtraction;
        public PropertyEdit propertyEdit;

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

        public void RefreshEdit(int lineID, int boxID)
        {
            this.lineID = lineID < 0 ? this.lineID : lineID;
            thisText.text = $"线号：{this.lineID}";
        }

        private void RefreshNote()
        {
            /*
            foreach (LabelItem item in propertyEdit.labelWindow.associateLabelWindow.labels)
            {
                if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
                {
                    NoteEdit.NoteEdit noteEdit = (NoteEdit.NoteEdit)item.labelWindowContent;
                    noteEdit.RefreshEdit(lineID, -1);
                }
            }*/
            GlobalData.Refresh<IRefreshEdit>(a => a.RefreshEdit(lineID, -1),
                new List<Type> { typeof(NoteEdit.NoteEdit), typeof(EventEdit.EventEdit) });
        }
    }
}