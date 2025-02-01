using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = Data.ChartEdit.Event;
namespace Form.NotePropertyEdit.ValueEdit
{
    public class EditEvent : MonoBehaviour
    {
        public Event @event;

        public TMP_InputField startTime;
        public TMP_InputField endTime;
        public TMP_InputField startValue;
        public TMP_InputField endValue;
        public Toggle syncEvent;
        public TMP_InputField easeIndex;
        public TMP_Dropdown easeIO;
        public TMP_Dropdown ease;
    }
}
