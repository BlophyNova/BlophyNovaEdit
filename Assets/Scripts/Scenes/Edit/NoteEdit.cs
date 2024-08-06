using Data.ChartEdit;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scenes.Edit
{
    public class NoteEdit : PublicButton
    {
        public RectTransform thisNoteRect;
        public Note thisNoteData;
        public virtual NoteEdit Init(Note note)
        {
            thisNoteData = note;
            return this;
        }
        //public void Start()
        //{
        //    thisButton.onClick.AddListener(()=>Debug.Log("dfgburkhujfokjifokmejiokfjifeop"));
        //}
    }
}