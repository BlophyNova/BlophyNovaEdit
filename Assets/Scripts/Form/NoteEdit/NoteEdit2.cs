using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {

        public delegate void OnNoteDeleted(Scenes.Edit.NoteEdit noteEdit);
        public event OnNoteDeleted onNoteDeleted = noteEdit => { };
        void SelectBoxDown()
        {
            selectBox.isPressing = true;
            selectBox.transform.SetAsLastSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }
        void SelectBoxUp()
        {
            selectBox.isPressing = false;
            selectBox.transform.SetAsFirstSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        void UndoNote()
        {

        }

        void RedoNote()
        {

        }

        void CopyNote()
        {

        }

        void PasteNote()
        {

        }

        void CutNote()
        {

        }

        void MirrorNote()
        {
            Debug.Log("镜像音符");
        }
    }
}