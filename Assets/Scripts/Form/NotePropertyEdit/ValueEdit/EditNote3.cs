using Controller;
using Data.ChartData;
using Data.Interface;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditNote
    {
        private void Start()
        {
            noteType.onValueChanged.AddListener(value => 
            {
                foreach (Data.ChartEdit.Note note in notes)
                {
                    note.noteType=(NoteType)value;
                }
                GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1));
                GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1));
            });

        }
    }
}