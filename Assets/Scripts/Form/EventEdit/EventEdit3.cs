using Data.ChartData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventEdit
{
    public delegate void OnBoxRefreshed(object content);
    public event OnBoxRefreshed onBoxRefreshed = c => { };
}
