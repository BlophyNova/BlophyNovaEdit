using Form.LabelWindow;
using Form.NotePropertyEdit.ValueEdit;
using UnityEngine;

namespace Form.NotePropertyEdit
{
    public class NotePropertyEdit : LabelWindowContent
    {
        public delegate void OnEventValueChanged();

        public delegate void OnNoteValueChanged();

        [SerializeField] private EditNote editNote;
        [SerializeField] private EditEvent editEvent;

        public EditNote EditNote
        {
            get
            {
                labelItem.labelButton.ThisLabelGetFocus();
                return editNote;
            }
            private set => editNote = value;
        }

        public EditEvent EditEvent
        {
            get
            {
                labelItem.labelButton.ThisLabelGetFocus();
                return editEvent;
            }
            private set => editEvent = value;
        }

        private void Start()
        {
            UnsetAll();
        }

        public event OnNoteValueChanged onNoteValueChanged = () => { };
        public event OnEventValueChanged onEventValueChanged = () => { };

        public void UnsetAll()
        {
            editNote.gameObject.SetActive(false);
            editEvent.gameObject.SetActive(false);
        }
    }
}