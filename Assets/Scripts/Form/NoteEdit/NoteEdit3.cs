using Data.ChartData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        public delegate void OnBoxRefreshed(Box box, int boxID);
        public event OnBoxRefreshed onBoxRefreshed = (Box box, int boxID) => { };
    }
}