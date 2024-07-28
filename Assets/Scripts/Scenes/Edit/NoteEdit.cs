using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scenes.Edit
{
    public class NoteEdit : MonoBehaviour
    {
        public Note thisNoteData;
        public virtual void Init(Note note)
        {
            thisNoteData = note;
        }
    }
}