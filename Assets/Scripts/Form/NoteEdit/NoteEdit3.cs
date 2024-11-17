using Data.ChartData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        public delegate void OnBoxRefreshed(object content);
        public event OnBoxRefreshed onBoxRefreshed = c => { };
    }
}