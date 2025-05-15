using CustomSystem;
using Data.ChartData;
using Data.ChartEdit;
using Form.EventEdit;
using Form.LabelWindow;
using Form.NotePropertyEdit.ValueEdit;
using Log;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = Data.ChartEdit.Event;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;

namespace Form.NotePropertyEdit
{
    public partial class NotePropertyEdit : LabelWindowContent
    {
        public delegate void OnEventValueChanged();

        public delegate void OnNoteValueChanged();

        public event OnNoteValueChanged onNoteValueChanged = () => { };
        public event OnEventValueChanged onEventValueChanged = () => { };
        [SerializeField] EditNote editNote;
        [SerializeField] EditEvent editEvent;

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

            labelItem.onLabelLostFocus += UnsetAll;
        }

        public void UnsetAll()
        {
            editNote.gameObject.SetActive(false);
            editEvent.gameObject.SetActive(false);
        }

    }
}