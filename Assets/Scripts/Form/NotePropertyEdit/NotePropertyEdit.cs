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

        [SerializeField] EditNote editNote;
        [SerializeField] EditEvent editEvent;

        public EditNote EditNote 
        {
            get => editNote; set => editNote = value;
        }
        public EditEvent EditEvent
        {
            get => editEvent; set => editEvent = value;
        }

        public event OnNoteValueChanged onNoteValueChanged = () => { };
        public event OnEventValueChanged onEventValueChanged = () => { };

    }
}