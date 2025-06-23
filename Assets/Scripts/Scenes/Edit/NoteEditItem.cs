using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Log;
using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Edit
{
    public class NoteEditItem : PublicButton, ISelectBoxItem
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
            OnStart();
        }

        protected virtual void OnStart()
        {
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
            try
            {
                ((HoldEdit)this).SetStartAndEndVisibility(active);
            }
            catch
            {
                // ignored
            }
            thisNoteData.isSelected = active;
            isSelectedRect.gameObject.SetActive(active);
        }

        public float GetStartBeats()
        {
            return thisNoteData.HitBeats.ThisStartBPM;
        }

        public virtual void SetStartAndEndVisibility(bool visibility) { }

        public virtual NoteEditItem Init(Note note)
        {
            thisNoteData = note;
            //SetSelectState(note.isSelected);
            return this;
        }
    }
}