using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ChartTool;
public class ChartPreview : LabelWindowContent, IRefresh
{
    public void Refresh()
    {
        ProgressManager.Instance.OffsetTime(0);
    }
}
