using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scenes.Edit
{
    public class NoteEdit : MonoBehaviour
    {
        public RectTransform thisNoteRect;
        public Note thisNoteData;
        public virtual NoteEdit Init(Note note)
        {
            thisNoteData = note;
            return this;
        }
    }
}