using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class BoxID : MonoBehaviour, IRefresh
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

        public void Refresh()
        {
        }

        public void RefreshNote()
        {
            foreach (LabelItem item in propertyEdit.labelWindow.associateLabelWindow.labels)
            {
                if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.EventEdit)
                {
                    EventEdit.EventEdit eventEdit = (EventEdit.EventEdit)item.labelWindowContent;
                    eventEdit.RefreshEvents(boxID);
                }

                if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
                {
                    NoteEdit.NoteEdit noteEdit = (NoteEdit.NoteEdit)item.labelWindowContent;
                    noteEdit.RefreshNotes(boxID, -1);
                }
            }
        }
    }
}