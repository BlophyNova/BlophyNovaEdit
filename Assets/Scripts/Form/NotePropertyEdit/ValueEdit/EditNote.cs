using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Form.NotePropertyEdit.ValueEdit
{
    public class EditNote : MonoBehaviour
    {
        public Note note;

        public TMP_Dropdown noteType;
        public Toggle hitEffect;
        public Toggle hitRipple;
        public TMP_InputField startTime;
        public TMP_InputField holdTime;
        public TMP_InputField postionX;
        public Toggle isClockwise;
        public TMP_InputField speed;
        public Toggle isFakeNote;
    }
}