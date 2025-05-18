using System;
using System.Collections.Generic;
using Controller;
using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class BoxID : MonoBehaviour, IRefreshEdit
    {
        public int boxID;
        public TMP_Text thisText;
        public Button add;
        public Button subtraction;
        public PropertyEdit propertyEdit;

        private void Start()
        {
            add.onClick.AddListener(() =>
            {
                if (boxID + 1 >= GlobalData.Instance.chartEditData.boxes.Count)
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

        public void RefreshEdit(int lineID, int boxID)
        {
            this.boxID = boxID < 0 ? this.boxID : boxID;
            thisText.text = $"框号：{this.boxID}";
        }

        public void RefreshNote()
        {
            /*
                foreach (LabelItem item in propertyEdit.labelWindow.associateLabelWindow.labels)
                {
                    if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.EventEdit)
                    {
                        EventEdit.EventEdit eventEdit = (EventEdit.EventEdit)item.labelWindowContent;
                        eventEdit.RefreshEdit(-1,boxID);
                    }

                    if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
                    {
                        NoteEdit.NoteEdit noteEdit = (NoteEdit.NoteEdit)item.labelWindowContent;
                        noteEdit.RefreshEdit(-1, boxID);
                    }
                }*/
            GlobalData.Refresh<IRefreshEdit>(a => a.RefreshEdit(-1, boxID),
                new List<Type> { typeof(NoteEdit.NoteEdit), typeof(EventEdit.EventEdit) });
            GameController.Instance.ChangeShowXYPoint(boxID);
        }
    }
}