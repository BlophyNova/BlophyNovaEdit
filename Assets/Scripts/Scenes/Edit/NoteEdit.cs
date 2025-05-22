using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Log;
using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Edit
{
    public class NoteEdit : PublicButton, ISelectBoxItem
    {
        public LabelWindow labelWindow;
        public RectTransform thisNoteRect;
        public Note thisNoteData;

        public RectTransform isSelectedRect;

        //public Form.NoteEdit.NoteEdit ThisNoteEdit => (Form.NoteEdit.NoteEdit)labelWindow.currentLabelItem.labelWindowContent;
        private Form.NoteEdit.NoteEdit thisNoteEdit;

        public Form.NoteEdit.NoteEdit ThisNoteEdit
        {
            get
            {
                if (thisNoteEdit == null)
                {
                    foreach (LabelItem item in labelWindow.labels)
                    {
                        if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
                        {
                            thisNoteEdit = (Form.NoteEdit.NoteEdit)item.labelWindowContent;
                        }
                    }
                }

                return thisNoteEdit;
            }
        }

        private void Start()
        {
            thisButton.onClick.AddListener(() => { ThisNoteEdit.selectBox.SetSingleNote(this); });
        }

        public bool IsNoteEdit => true;

        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[4];
            thisNoteRect.GetWorldCorners(corners);
            return corners;
        }

        public void SetSelectState(bool active)
        {
            thisNoteData.isSelected = active;
            isSelectedRect.gameObject.SetActive(active);
            LogCenter.Log(
                $@"{ThisNoteEdit.currentBoxID}号框的{ThisNoteEdit.currentLineID}号线的{thisNoteData.HitBeats.integer}:{thisNoteData.HitBeats.molecule}/{thisNoteData.HitBeats.denominator}的选择状态被改为：{isSelectedRect.gameObject.activeSelf}");
        }

        public float GetStartBeats()
        {
            return thisNoteData.HitBeats.ThisStartBPM;
        }

        public virtual NoteEdit Init(Note note)
        {
            thisNoteData = note;
            //SetSelectState(note.isSelected);
            return this;
        }
    }
}