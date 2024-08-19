using Data.ChartEdit;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scenes.Edit
{
    public class NoteEdit : PublicButton
    {
        public LabelWindow labelWindow;
        public RectTransform thisNoteRect;
        public Note thisNoteData;
        public RectTransform isSelectedRect;
        public virtual NoteEdit Init(Note note)
        {
            thisNoteData = note;
            return this;
        }
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                if (labelWindow.associateLabelWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
                {
                    NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelWindow;
                    notePropertyEdit.SelectedNote(this);
                }
            });
        }
    }
}