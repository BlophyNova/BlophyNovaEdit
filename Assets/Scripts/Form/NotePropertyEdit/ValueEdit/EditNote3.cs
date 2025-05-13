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
                
            });

        }
    }
}