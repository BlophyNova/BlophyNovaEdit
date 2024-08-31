using Data.ChartEdit;
using Form.NotePropertyEdit;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scenes.Edit
{
    public class NoteEdit : PublicButton, ISelectBoxItem
    {
        public LabelWindow labelWindow;
        public RectTransform thisNoteRect;
        public Note thisNoteData;
        public RectTransform isSelectedRect;
        public Form.NoteEdit.NoteEdit ThisNoteEdit => (Form.NoteEdit.NoteEdit)labelWindow.currentLabelWindow;
        public bool IsNoteEdit => true;
        public virtual NoteEdit Init(Note note)
        {
            thisNoteData = note;
            SetSelectState(false);
            return this;
        }
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                ThisNoteEdit.selectBox.SetSingleNote(this);
            });
        }
        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[4];
            thisNoteRect.GetWorldCorners(corners);
            return corners;
        }

        public void SetSelectState(bool active)
        {
            isSelectedRect.gameObject.SetActive(active);
        }
    }
}