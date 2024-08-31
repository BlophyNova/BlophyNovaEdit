using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        void SelectBoxDown()
        {
            selectBox.isPressing = true;
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }
        void SelectBoxUp()
        {
            selectBox.isPressing = false;
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }
    }
}