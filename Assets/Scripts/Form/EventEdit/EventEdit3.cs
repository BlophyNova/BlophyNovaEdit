using Data.ChartData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventEdit
{
    public delegate void OnBoxRefreshed(Box box, int boxID);
    public event OnBoxRefreshed onBoxRefreshed = (Box box, int boxID) => { };
}
